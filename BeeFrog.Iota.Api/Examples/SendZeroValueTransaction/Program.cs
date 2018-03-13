namespace SendZeroValueTransaction
{
    using System;
    using System.Linq;
    using System.Threading;
    using BeeFrog.Iota.Api;
    using BeeFrog.Iota.Api.Models;
    using BeeFrog.Iota.Api.NodeLookup;

    /// <summary>
    /// This example shows you how to create a Zero value transaction and post it to the Tangle.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Finding an IOTA node");
            INodeFinder nodeFinder = new IotaDanceNodeFinder();
            var nodes = nodeFinder.FindNodes().Result;
            var bestNodeUrl = nodes.ElementAt(1).Url;

            Console.WriteLine($"Creating transfer now. This may take a few seconds.");
            var transferItem = new TransferItem() { Address = "FAKEADDRESS9999999999999999999999999999999999999999999999999999999999999999999999", Message = "TESTMESSAGE" };
            var api = new IotaApi(bestNodeUrl);


            var apiResult = api.AttachTransfer(transferItem, CancellationToken.None).Result;

            if (apiResult.Successful)
            {
                var transactions = apiResult.Result;
                Console.WriteLine($"Your transaction hash is: {transactions[0].Hash}");
            }
            else
            {
                Console.WriteLine($"{apiResult.ErrorMessage} Exception: {apiResult.ExceptionInfo}");
            }

            Console.WriteLine($"Press any key to close");
            Console.ReadKey();
        }
    }
}
