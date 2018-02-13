namespace CreateAndPromoteATransaction
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using BeeFrog.Iota.Api;
    using BeeFrog.Iota.Api.Models;
    using BeeFrog.Iota.Api.NodeLookup;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Finding an IOTA node");
            INodeFinder nodeFinder = new IotaDanceNodeFinder();
            var nodes = nodeFinder.FindNodes().Result;
            var bestNodeUrl = nodes.ElementAt(1).Url;

            var api = new IotaApi(bestNodeUrl);
            
            Console.WriteLine($"Creating transfer now. This may take a few seconds.");
            var transferItem = new TransferItem() { Address = "FAKEADDRESS9999999999999999999999999999999999999999999999999999999999999999999999", Message = "PROMOTE9TEST" };
            var transaction = api.AttachTransfer(transferItem, CancellationToken.None).Result;
            var hash = transaction[0].Hash;
            Console.WriteLine($"Your transaction hash is: {hash}");

            Promote(api, hash);

            Console.WriteLine($"All done.");
            Console.WriteLine($"Press any key to close");
            Console.ReadKey();            
        }

        private static void Promote(IotaApi api, string hash)
        {
            while (true)
            {
                Console.WriteLine("Waiting 60 seconds before promoting. Press any key to stop.");
                for (int i = 0; i < 240; i++)
                {
                    if (i % 4 == 0) { Console.Write("."); }

                    Thread.Sleep(250);
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey();
                        return;
                    }
                }

                var states = api.IriApi.GetInclusionStates(new[] { hash }).Result;
                if(states.Any() && states[0])
                {
                    Console.WriteLine("Your transaction has been confirmed!");
                    return;
                }

                var ok = api.IriApi.CheckConsistency(hash, hash).Result;
                if (!ok)
                {
                    Console.WriteLine("Your transaction needs to be re-attached!");

                    var result = api.GetTransactionItems(hash).Result;
                    Console.WriteLine("Re-attaching!");
                    var newTran = api.AttachTransactions(result, CancellationToken.None).Result;
                    hash = newTran[0].Hash;
                    continue;
                }

                Console.WriteLine(" Promoting.");
                try
                {
                    var tran = api.PromoteTransaction(hash, CancellationToken.None).Result;
                    Console.WriteLine(tran.Hash);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Promotion failed!");
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
