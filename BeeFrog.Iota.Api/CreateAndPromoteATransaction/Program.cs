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
            var transferItem = new TransferItem() { Address = "FAKEADDRESS9999999999999999999999999999999999999999999999999999999999999999999999", Message = "TESTMESSAGE" };
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
                Console.WriteLine("Waiting 10 seconds before promoting. Press any key to stop.");
                for (int i = 0; i < 40; i++)
                {
                    if (i % 4 == 0) { Console.Write("."); }

                    Thread.Sleep(250);
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey();
                        return;
                    }
                }

                Console.WriteLine(" Promoting.");
                var tran = api.PromoteTransaction(hash, CancellationToken.None).Result;
                Console.WriteLine(tran.Hash);
            }
        }
    }
}
