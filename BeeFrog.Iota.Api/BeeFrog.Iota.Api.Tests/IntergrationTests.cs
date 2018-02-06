﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using BeeFrog.Iota.Api.Models;
using BeeFrog.Iota.Api.Utils;
using NUnit.Framework;

namespace BeeFrog.Iota.Api.Tests
{

    /// <summary>
    /// These tests are likly to be longer running tests which may actually hit the API.
    /// They will be flacky by nature as they can be affected by external factors such as node availability and Internet connectivity.
    /// </summary>
    [TestFixture, Explicit]
    public class IntergrationTests
    {
        [Test]
        public void SendZeroTran()
        {
            var api = new IotaApi("https://nodes.thetangle.org:443");
            var tran = new TransferItem() { Address = "S9XDEUXLTPUKULJXMV9IUKBMLGRHLXPCIMWFE9UHIGQGJEZSJRPYFBDSNNSMDEHBSTUEGAGBX9QZNKDNY", Tag = "UNITTEST" };

            var result = api.AttachTransfer(tran, CancellationToken.None).Result;
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetTrytesMine22()
        {
            var trytes = "999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999S9XDEUXLTPUKULJXMV9IUKBMLGRHLXPCIMWFE9UHIGQGJEZSJRPYFBDSNNSMDEHBSTUEGAGBX9QZNKDNY999999999999999999999999999IGYNONE99999999999999999999IEJLTYD99999999999999999999XX9TDOEWQGKQNJTZPFAJJLOREYICWGHIIZEQFFFBACSYTKQJBPJVNJTHRAUKQT9AEWVGXACCVZFJICPJWT9LVYFQGF9WQHDQNLHPAKNFAC9VXPHNJPDFIQIELAYMBHCPDQBQSSXPLZRVSVSQHGPXKMVNGKRWN99999A9VWVUZWGPPYCBOTNJZAQMUDWZVXFLTCQZZJHWGSOAUTXPSMEFJQZKOKAIIJYMPYLUSPODTIL9WS99999PAYNONE99999999999999999999LASLSZBJE999999999MMMMMMMMMCAMDTIYKYAYPIZOMMTZAFODTEIS";

            var api = new IotaApi("https://nodes.thetangle.org:443");
            var bundle = api.GetBundleTransactionItems("XX9TDOEWQGKQNJTZPFAJJLOREYICWGHIIZEQFFFBACSYTKQJBPJVNJTHRAUKQT9AEWVGXACCVZFJICPJW").Result;
            var tran = bundle[0];
            Assert.AreEqual(trytes, tran.RawTrytes);
        }

        [Test]
        public void GetTrytes()
        {
            var api = new IotaApi("https://nodes.thetangle.org:443");
            var bundle = api.GetBundleTransactionItems("NLAVEPNFGLLJXER9VKTTBQSHTXZGBLNH9ILHB9TXH9YEGGETCQAWIYPKKWSAODFORD9TWJHPHLNYHWIQX").Result;
            foreach (var tran in bundle)
            {
                var rTrytes = tran.RawTrytes;
                var x = tran.ToString();
            }
        }

        [Test]
        public void CreateAddresses_FromSeed_CorrectAddressesReturned()
        {
            var seed = "ABM9ADN999VDXCHYJXDKUQITXAWQPBWZGYTBGTEIFWXOZTMHESEVHYLXWASWQFEJHUAKHIKSCA9AL9KMG";

            for (int i = 0; i < 10; i++)
            {
                var address = IotaUtils.GenerateAddress(seed, i, 2, System.Threading.CancellationToken.None);
                Console.WriteLine($"{i}: {address.AddressWithCheckSum}");
            }
        }
    }
}
