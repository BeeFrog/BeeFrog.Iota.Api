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
            var node = nodeFinder.GetBestNode().Result;
            var api = new IotaApi(node);
            Console.WriteLine("Found node: " + node);

            Console.WriteLine($"Creating transfer now. This may take a few seconds.");
            var transferItem = new TransferItem() { Address = "FAKEADDRESS9999999999999999999999999999999999999999999999999999999999999999999999", Message = "PROMOTE9TEST" };
            var response = api.AttachTransfer(transferItem, CancellationToken.None).Result;
            if (response.Successful)
            {
                var transaction = response.Result;
                var hash = transaction[0].Hash;
                Console.WriteLine($"Your transaction hash is: {hash}");

                Promote(api, hash);
            }
            else
            {
                Console.WriteLine($"Error Create Transaction: {response.ErrorMessage} Exception: {response.ExceptionInfo}");
            }

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
                if(states.Successful && states.Result.Any() && states.Result[0])
                {
                    Console.WriteLine("Your transaction has been confirmed!");
                    return;
                }

                var consistencyResponse = api.IriApi.CheckConsistency(hash, hash).Result;
                if (consistencyResponse.Successful)
                {
                    if (consistencyResponse.Result == false)
                    {
                        Console.WriteLine("Your transaction needs to be re-attached!");
                        var response = api.GetTransactionItems(hash).Result;
                        if (response.Successful)
                        {
                            Console.WriteLine("Re-attaching!");
                            var reattachResponse = api.AttachTransactions(response.Result, CancellationToken.None).Result;
                            if (reattachResponse.Successful)
                            {
                                hash = reattachResponse.Result[0].Hash;
                                Console.WriteLine("Success! Your new hash is:" + hash);
                                continue;
                            }
                            else
                            {
                                Console.WriteLine($"Re-attach Failed! Reason: {reattachResponse.ErrorMessage} Exception:{reattachResponse.ExceptionInfo}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Get Transaction Failed! Reason: {response.ErrorMessage} Exception:{response.ExceptionInfo}");
                        }
                    }

                    Console.WriteLine(" Promoting.");
                    var promoteResponse = api.PromoteTransaction(hash, CancellationToken.None).Result;
                    if (promoteResponse.Successful)
                    {
                        Console.WriteLine(promoteResponse.Result.Hash);
                    }
                    else
                    {
                        Console.WriteLine($"Promotion failed!! Reason: {promoteResponse.ErrorMessage} Exception:{promoteResponse.ExceptionInfo}");
                    }
                }
                else
                {
                    Console.WriteLine($"CheckConsistency failed! Reason: {consistencyResponse.ErrorMessage} Exception:{consistencyResponse.ExceptionInfo}");
                }
            }
        }
    }
}
