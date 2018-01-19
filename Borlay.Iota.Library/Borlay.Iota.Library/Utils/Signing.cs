using Borlay.Iota.Library.Crypto;
using Borlay.Iota.Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Borlay.Iota.Library.Utils
{
    /// <summary>
    /// Ask cfb
    /// </summary>
    public class Signing
    {
        private ICurl curl;

        private const int HASH_LENGTH = 243;
        private int STATE_LENGTH = 3 * HASH_LENGTH;
        private const int NUMBER_OF_ROUNDS = 27;


        public Signing(ICurl curl)
        {
            this.curl = curl;
        }

        public Signing()
        {
            this.curl = new Curl();
        }

        public int[] Key(int[] seed, int index, int length)
        {
            int[] subseed = seed;

            for (int i = 0; i < index; i++)
            {
                for (int j = 0; j < HASH_LENGTH; j++)
                {
                    if (++subseed[j] > 1)
                    {
                        subseed[j] = -1;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            curl.Reset();
            curl.Absorb(subseed, 0, subseed.Length);
            curl.Squeeze(subseed, 0, subseed.Length);
            curl.Reset();
            curl.Absorb(subseed, 0, subseed.Length);

            IList<int> key = new List<int>();
            int[] buffer = new int[subseed.Length];
            int offset = 0;

            while (length-- > 0)
            {
                for (int i = 0; i < NUMBER_OF_ROUNDS; i++)
                {
                    curl.Squeeze(buffer, offset, buffer.Length);
                    for (int j = 0; j < HASH_LENGTH; j++)
                    {
                        key.Add(buffer[j]);
                    }
                }
            }
            return ToIntArray(key);
        }

        private static int[] ToIntArray(IList<int> key)
        {
            int[] a = new int[key.Count];
            int i = 0;
            foreach (int v in key)
            {
                a[i++] = v;
            }
            return a;
        }

        public int[] Digests(int[] key)
        {
            int[] digests = new int[(int)Math.Floor((decimal)key.Length / 6561) * HASH_LENGTH];
            int[] buffer = new int[HASH_LENGTH];
            
            for (int i = 0; i < Math.Floor((decimal)key.Length / 6561); i++)
            {
                int[] keyFragment = new int[6561];
                Array.Copy(key, i * 6561, keyFragment, 0, 6561);

                for (int j = 0; j < NUMBER_OF_ROUNDS; j++)
                {
                    
                    Array.Copy(keyFragment, j * HASH_LENGTH, buffer, 0, HASH_LENGTH);
                    for (int k = 0; k < 26; k++)
                    {
                        curl.Reset();
                        curl.Absorb(buffer, 0, buffer.Length);
                        curl.Squeeze(buffer, 0, buffer.Length);
                    }
                    for (int k = 0; k < HASH_LENGTH; k++)
                    {
                        keyFragment[j * HASH_LENGTH + k] = buffer[k];
                    }
                }

                curl.Reset();
                curl.Absorb(keyFragment, 0, keyFragment.Length);
                curl.Squeeze(buffer, 0, buffer.Length);

                for (int j = 0; j < HASH_LENGTH; j++)
                {
                    digests[i * HASH_LENGTH + j] = buffer[j];
                }
            }
            
            return digests;
        }

        public int[] Digest(int[] normalizedBundleFragment, int[] signatureFragment)
        {
            curl.Reset();
            int[] buffer = new int[HASH_LENGTH];

            for (int i = 0; i < NUMBER_OF_ROUNDS; i++)
            {
                buffer = ArrayUtils.SubArray(signatureFragment, i * HASH_LENGTH, HASH_LENGTH);
                ;
                ICurl jCurl = curl.Clone();

                for (int j = normalizedBundleFragment[i] + 13; j-- > 0;)
                {
                    jCurl.Reset();
                    jCurl.Absorb(buffer);
                    jCurl.Squeeze(buffer);
                }
                curl.Absorb(buffer);
            }
            curl.Squeeze(buffer);

            return buffer;
        }

        public int[] Address(int[] digests)
        {
            int[] address = new int[HASH_LENGTH];
            curl.Reset()
                .Absorb(digests, 0, digests.Length)
                .Squeeze(address, 0, address.Length);
            return address;
        }

        public int[] SignatureFragment(int[] normalizedBundleFragment, int[] keyFragment)
        {
            int[] hash = new int[HASH_LENGTH];

            for (int i = 0; i < NUMBER_OF_ROUNDS; i++)
            {
                Array.Copy(keyFragment, i * HASH_LENGTH, hash, 0, HASH_LENGTH);

                for (int j = 0; j < 13 - normalizedBundleFragment[i]; j++)
                {
                    curl.Reset()
                        .Absorb(hash, 0, hash.Length)
                        .Squeeze(hash, 0, hash.Length);
                }

                for (int j = 0; j < HASH_LENGTH; j++)
                {
                    Array.Copy(hash, j, keyFragment, i * HASH_LENGTH + j, 1);
                }
            }

            return keyFragment;
        }

        public bool ValidateSignatures(string expectedAddress, string[] signatureFragments, string bundleHash)
        {
            //Bundle bundle = new Bundle();

            var normalizedBundleFragments = new int[3, NUMBER_OF_ROUNDS];
            int[] normalizedBundleHash = TransactionExtensions.NormalizedBundle(bundleHash); //bundle.NormalizedBundle(bundleHash);

            // Split hash into 3 fragments
            for (int i = 0; i < 3; i++)
            {
                // normalizedBundleFragments[i] = Arrays.copyOfRange(normalizedBundleHash, i*NUMBER_OF_ROUNDS, (i + 1)*NUMBER_OF_ROUNDS);
                Array.Copy(normalizedBundleHash, i * NUMBER_OF_ROUNDS, normalizedBundleFragments, 0, NUMBER_OF_ROUNDS);
            }

            // Get digests
            int[] digests = new int[signatureFragments.Length * HASH_LENGTH];

            for (int i = 0; i < signatureFragments.Length; i++)
            {
                int[] digestBuffer = Digest(ArrayUtils.SliceRow(normalizedBundleFragments, i % 3).ToArray(),
                    Converter.ToTrits(signatureFragments[i]));

                for (int j = 0; j < HASH_LENGTH; j++)
                {
                    Array.Copy(digestBuffer, j, digests, i * HASH_LENGTH + j, 1);
                }
            }
            string address = Converter.ToTrytes(Address(digests));

            return (expectedAddress.Equals(address));
        }
    }
}
