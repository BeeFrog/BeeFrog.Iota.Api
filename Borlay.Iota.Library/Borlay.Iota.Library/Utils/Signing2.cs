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
    public class Signing2
    {
        public readonly static int KEY_LENGTH = 6561;
        private const int NUMBER_OF_ROUNDS = 27;
        private ICurl curl;

        /**
         * public Signing() {
         * this(null);
         * }
         *
         * /**
         *
         * @param curl
         */
        public Signing2(ICurl curl)
        {
            this.curl = curl == null ? this.GetICurlObject() : curl;
        }

        /**
         * @param inSeed
         * @param index
         * @param security
         * @return
         * @throws ArgumentException is thrown when the specified security level is not valid.
         */
        public int[] key(int[] inSeed, int index, int security)
        {
            if (security < 1) {
                throw new ArgumentException(Constants.INVALID_SECURITY_LEVEL_INPUT_ERROR);
            }

            int[] seed = (int[])inSeed.Clone();

            // Derive subseed.
            for (int i = 0; i < index; i++) {
                for (int j = 0; j < seed.Length; j++) {
                    if (++seed[j] > 1) {
                        seed[j] = -1;
                    } else {
                        break;
                    }
                }
            }

            ICurl curl = this.GetICurlObject();
            curl.Reset();
            curl.Absorb(seed, 0, seed.Length);
            // seed[0..HASH_LENGTH] contains subseed
            curl.Squeeze(seed, 0, seed.Length);
            curl.Reset();
            // absorb subseed
            curl.Absorb(seed, 0, seed.Length);

            int[] key = new int[security * Curl.HashLength * 27];  //TODO is this not 81!!
            int[] buffer = new int[seed.Length];
            int offset = 0;

            while (security-- > 0) {
                for (int i = 0; i < 27; i++) {
                    curl.Squeeze(buffer, 0, seed.Length);

                    Array.Copy(buffer, 0, key, offset, Curl.HashLength);
                    //System.arraycopy(buffer, 0, key, offset, Curl.HashLength);

                    offset += Curl.HashLength;
                }
            }
            return key;
        }

        public int[] signatureFragment(int[] normalizedBundleFragment, int[] keyFragment)
        {
            int[] signatureFragment = (int[])keyFragment.Clone();
            
            for (int i = 0; i < 27; i++)
            {

                for (int j = 0; j < 13 - normalizedBundleFragment[i]; j++)
                {
                    curl.Reset()
                            .Absorb(signatureFragment, i * Curl.HashLength, Curl.HashLength)
                            .Squeeze(signatureFragment, i * Curl.HashLength, Curl.HashLength);
                }
            }

            return signatureFragment;
        }

        public int[] address(int[] digests)
        {
            int[] address = new int[Curl.HashLength];
            ICurl curl = this.GetICurlObject();
            curl.Reset()
                .Absorb(digests)
                .Squeeze(address);

            return address;
        }

        public int[] digests(int[] key)
        {
            int security = (int)Math.Floor((double)key.Length / KEY_LENGTH);

            int[] digests = new int[security * Curl.HashLength];
            int[] keyFragment = new int[KEY_LENGTH];

            ICurl curl = this.GetICurlObject();
            for (int i = 0; i < Math.Floor((double)key.Length / KEY_LENGTH); i++)
            {
                Array.Copy(key, i * KEY_LENGTH, keyFragment, 0, KEY_LENGTH);
                //System.arraycopy(key, i * KEY_LENGTH, keyFragment, 0, KEY_LENGTH);

                for (int j = 0; j < 27; j++)
                {
                    for (int k = 0; k < 26; k++)
                    {
                        curl.Reset()
                                .Absorb(keyFragment, j * Curl.HashLength, Curl.HashLength)
                                .Squeeze(keyFragment, j * Curl.HashLength, Curl.HashLength);
                    }
                }

                curl.Reset();
                curl.Absorb(keyFragment, 0, keyFragment.Length);
                curl.Squeeze(digests, i * Curl.HashLength, Curl.HashLength);
            }
            return digests;
        }

        public int[] digest(int[] normalizedBundleFragment, int[] signatureFragment)
        {
            this.curl.Reset();
            ICurl curl = this.GetICurlObject();
            int[] buffer = new int[Curl.HashLength];

            for (int i = 0; i < 27; i++)
            {
                Array.Copy(signatureFragment, i * Curl.HashLength, buffer, 0, Curl.HashLength);

                // buffer = Array.Copy(signatureFragment, i * Curl.HashLength, buffer, (i + 1) * Curl.HashLength, (i + 1) * NUMBER_OF_ROUNDS);

                for (int j = normalizedBundleFragment[i] + 13; j-- > 0;)
                {
                    curl.Reset();
                    curl.Absorb(buffer);
                    curl.Squeeze(buffer);
                }
                this.curl.Absorb(buffer);
            }
            this.curl.Squeeze(buffer);

            return buffer;
        }

        private ICurl GetICurlObject()
        {            
            return new Kerl();
        }

        public Boolean validateSignatures(String expectedAddress, String[] signatureFragments, String bundleHash)
        {

            Bundle bundle = new Bundle();

            var normalizedBundleFragments = new int[3, 27];
            int[] normalizedBundleHash = TransactionExtensions.NormalizedBundle(bundleHash);

            // Split hash into 3 fragments
            for (int i = 0; i < 3; i++)
            {
                Array.Copy(normalizedBundleHash, i * 27, normalizedBundleFragments, i * 27, (i + 1) * 27);
                //normalizedBundleFragments[i] = Arrays.copyOfRange(normalizedBundleHash, i * 27, (i + 1) * 27);
            }

            // Get digests
            int[] digests = new int[signatureFragments.Length * Curl.HashLength];

            for (int i = 0; i < signatureFragments.Length; i++)
            {

                int[] digestBuffer = digest(ArrayUtils.SliceRow(normalizedBundleFragments, i % 3).ToArray(),
                    Converter.ToTrits(signatureFragments[i]));

                Array.Copy(digestBuffer, 0, digests, i * Curl.HashLength, Curl.HashLength);
            }
            string address = Converter.ToTrytes(this.Address(digests));

            return (expectedAddress.Equals(address));
        }

        private int[] Address(int[] digests)
        {
            int[] address = new int[Curl.HashLength];
            curl.Reset().Absorb(digests, 0, digests.Length).Squeeze(address, 0, address.Length);
            return address;
        }
    }
}
