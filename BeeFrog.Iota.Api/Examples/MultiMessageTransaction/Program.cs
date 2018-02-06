namespace MultiMessageTransaction
{
    using System;
    using System.Linq;
    using System.Threading;
    using BeeFrog.Iota.Api;
    using BeeFrog.Iota.Api.Crypto;
    using BeeFrog.Iota.Api.Models;
    using BeeFrog.Iota.Api.NodeLookup;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Finding an IOTA node");
            INodeFinder nodeFinder = new IotaDanceNodeFinder();
            var node = nodeFinder.FindNodes().Result.First();            

            Console.WriteLine($"Creating transfer now. This may take a few seconds.");

            // We'll create out 2 transactions.
            var transaction1 = new TransactionItem("FAKEADDRESS9999999999999999999999999999999999999999999999999999999999999999999999", 0, "BEEFROG")
            {
                SignatureFragment = BeeFrog.Iota.Api.Utils.Converter.AsciiToTrytes("Message part 1")
            };

            var transaction2 = new TransactionItem("FAKEADDRESS9999999999999999999999999999999999999999999999999999999999999999999999", 0, "BEEFROG")
            {
                SignatureFragment = BeeFrog.Iota.Api.Utils.Converter.AsciiToTrytes("Message part 2")
            };

            // Create an array and bundle the transactions together.
            var trans = new TransactionItem[] { transaction1, transaction2 };
            trans.FinalizeBundleHash(new Curl());

            // Now we can send them.
            var api = new IotaApi(node.Url);
            var transaction = api.AttachTransactions(trans, CancellationToken.None).Result;

            Console.WriteLine($"Your transaction bundle is: {transaction[0].Bundle}");

            Console.WriteLine($"Press any key to close");
            Console.ReadKey();
        }
    }
}
