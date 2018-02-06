﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BeeFrog.Iota.Api.Crypto
{
    public class Signing
    {
        public sbyte[] Key(sbyte[] seed, int index, int length)
        {
            if ((seed.Length % 243) != 0) 
            {
                var listSeed = seed.ToList();
                while ((listSeed.Count % 243) != 0)
                {
                    listSeed.Add(0);
                }
                seed = listSeed.ToArray();
            }

            var indexTrits = Utils.Converter.GetTritsFromInt(index);
            var subseed = Adder.Add(seed, indexTrits);

            var kerl = new Kerl();

            kerl.Initialize();
            kerl.Absorb(subseed, 0, subseed.Length);
            kerl.Squeeze(subseed, 0, subseed.Length);

            kerl.Reset();
            kerl.Absorb(subseed, 0, subseed.Length);

            IList<sbyte> key = new List<sbyte>();
            sbyte[] buffer = new sbyte[subseed.Length];
            int offset = 0;

            while (length-- > 0)
            {
                for (int i = 0; i < 27; i++)
                {
                    kerl.Squeeze(buffer, offset, buffer.Length);
                    for (int j = 0; j < 243; j++)
                    {
                        key.Add(buffer[j]);
                    }
                }
            }
            return key.ToArray();
        }

        public sbyte[] Digests(sbyte[] key)
        {
            var fragments = (int)Math.Floor((decimal)(key.Length / 6561));
            var digests = new sbyte[fragments * 243];
            sbyte[] buffer = null;

            for (var i = 0; i < fragments; i++)
            {

                var keyFragment = key.Slice(i * 6561, (i + 1) * 6561);

                for (var j = 0; j < 27; j++)
                {

                    buffer = keyFragment.Slice(j * 243, (j + 1) * 243);

                    for (var k = 0; k < 26; k++)
                    {

                        var kKerl = new Kerl();
                        kKerl.Initialize();
                        kKerl.Absorb(buffer, 0, buffer.Length);
                        kKerl.Squeeze(buffer, 0, Curl.HashLength);
                    }

                    for (var k = 0; k < 243; k++)
                    {

                        keyFragment[j * 243 + k] = buffer[k];
                    }
                }

                var kerl = new Kerl();

                kerl.Initialize();
                kerl.Absorb(keyFragment, 0, keyFragment.Length);
                kerl.Squeeze(buffer, 0, Curl.HashLength);

                for (var j = 0; j < 243; j++)
                {
                    digests[i * 243 + j] = buffer[j];
                }
            }

            return digests;
        }

        public sbyte[] Address(sbyte[] digests)
        {
            var addressTrits = new sbyte[Curl.HashLength];

            var kerl = new Kerl();

            kerl.Initialize();
            kerl.Absorb(digests, 0, digests.Length);
            kerl.Squeeze(addressTrits, 0, Curl.HashLength);

            return addressTrits;
        }
    }
}
