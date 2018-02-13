using System;
using System.Collections.Generic;
using System.Text;
using BeeFrog.Iota.Api.Utils;

namespace BeeFrog.Iota.Api.Crypto
{
    public class Checksum
    {
        public static string AddChecksum(string inputValue, int checksumLength = 9)
        {
            var kerl = new Kerl();
            kerl.Initialize();

            // Address trits
            var addressTrits = Converter.GetTrits(inputValue);

            // Checksum trits
            var checksumTrits = new sbyte[Curl.HashLength];

            // Absorb address trits
            kerl.Absorb(addressTrits, 0, addressTrits.Length);

            // Squeeze checksum trits
            kerl.Squeeze(checksumTrits, 0, Curl.HashLength);

            // First 9 trytes as checksum
            var checksum = Converter.GetTrytes(checksumTrits).Substring(81 - checksumLength, checksumLength);
            return inputValue + checksum;
        }

        public static string RemoveChecksum(string inputValue)
        {
            return inputValue.Substring(0, 81);
        }

        public static bool IsValidChecksum(string inputValue)
        {
            var withoutChecksum = RemoveChecksum(inputValue);
            var cLen = inputValue.Length - withoutChecksum.Length;
            var withChecksum = AddChecksum(withoutChecksum, cLen);
            return inputValue == withChecksum;
        }
    }
}