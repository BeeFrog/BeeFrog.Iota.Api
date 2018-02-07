using BeeFrog.Iota.Api.Crypto;
using BeeFrog.Iota.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeeFrog.Iota.Api.Models
{
    public static class TransactionExtensions
    {
        /// <summary>
        /// Calculates and sets the bundle has when there is only a single transaction in the bundle.
        /// </summary>
        /// <param name="transactionItems">The transaction item to bundle.</param>
        /// <returns>The bundle hash.</returns>
        public static string FinalizeBundleHash(this TransactionItem transactionItem)
        {
            return FinalizeBundleHash(new[] { transactionItem }, new Kerl());
        }

        /// <summary>
        /// Calculates the bundle hash for all transactions and sets the Bundle hash on each transaction.
        /// Index will also be set here too.
        /// </summary>
        /// <param name="transactionItems">The transaction items to bundle.</param>
        /// <returns>The bundle hash.</returns>
        public static string FinalizeBundleHash(this IEnumerable<TransactionItem> transactionItems)
        {
            return FinalizeBundleHash(transactionItems, new Kerl());
        }

        public static string FinalizeBundleHash(this IEnumerable<TransactionItem> transactionItems, ICurl customCurl)
        {
            customCurl.Reset();
            var transactionCount = transactionItems.Count();
            bool valid = false;
            string hashInTrytes= "";

            while (!valid)
            {
                for (int i = 0; i < transactionCount; i++)
                {
                    var transaction = transactionItems.ElementAt(i);
                    transaction.CurrentIndex = i;
                    transaction.LastIndex = transactionCount - 1;

                    var trytes = transaction.GetBundleTrytes();

                    int[] t = Converter.ToTrits(trytes);
                    customCurl.Absorb(t, 0, t.Length);
                }

                int[] hash = new int[243];
                customCurl.Squeeze(hash, 0, hash.Length);
                hashInTrytes = Converter.ToTrytes(hash);

                bool found = false;
                var normalizedBundleValues = NormalizedBundle(hashInTrytes);
                foreach (int normalizedBundleValue in normalizedBundleValues)
                {
                    if (normalizedBundleValue == 13)
                    {
                        found = true;
                        var obsoleteTagTrits = Converter.ToTritsString(transactionItems.First().ObsoleteTag);
                        Converter.Increment(obsoleteTagTrits, 81);
                        transactionItems.First().ObsoleteTag = Converter.ToTrytes(obsoleteTagTrits);
                        customCurl.Reset();
                    }
                }
                valid = !found;
            }

            foreach (var transaction in transactionItems)
            {
                transaction.Bundle = hashInTrytes;
            }

            return hashInTrytes;
        }

        public static int[] FinalizeAndNormalizeBundleHash(this IEnumerable<TransactionItem> transactionItems, ICurl customCurl)
        {
            var bundleHash = transactionItems.FinalizeBundleHash(customCurl);
            return NormalizedBundle(bundleHash);
        }

        /// <summary>
        /// Normalizeds the bundle.
        /// </summary>
        /// <param name="bundleHash">The bundle hash.</param>
        /// <returns></returns>
        public static int[] NormalizedBundle(string bundleHash)
        {
            int[] normalizedBundle = new int[81];

            for (int i = 0; i < 3; i++)
            {
                long sum = 0;
                for (int j = 0; j < 27; j++)
                {
                    sum +=
                    (normalizedBundle[i * 27 + j] =
                        Converter.ToValue(Converter.ToTritsString("" + bundleHash[i * 27 + j])));
                }

                if (sum >= 0)
                {
                    while (sum-- > 0)
                    {
                        for (int j = 0; j < 27; j++)
                        {
                            if (normalizedBundle[i * 27 + j] > -13)
                            {
                                normalizedBundle[i * 27 + j]--;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    while (sum++ < 0)
                    {
                        for (int j = 0; j < 27; j++)
                        {
                            if (normalizedBundle[i * 27 + j] < 13)
                            {
                                normalizedBundle[i * 27 + j]++;
                                break;
                            }
                        }
                    }
                }
            }

            return normalizedBundle;
        }
    }
}
