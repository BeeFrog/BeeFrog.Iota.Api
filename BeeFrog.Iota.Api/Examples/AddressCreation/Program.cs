namespace AddressCreation
{
    using System;
    using System.Threading;
    using BeeFrog.Iota.Api;

    class Program
    {
        static void Main(string[] args)
        {
            // Your seed is an 81 char TRYTE string. The Tryte alphabet is upper care A-Z and 9. Use a tool to create your own seed.
            // The seed below is the full alphabet repeated 3 times. NEVER user this seed to transfer Iota as your IOTA will not be safe.
            var seed = "ABCDEFGHIJKLMNOPQRSTUVWXYZ9ABCDEFGHIJKLMNOPQRSTUVWXYZ9ABCDEFGHIJKLMNOPQRSTUVWXYZ9";

            // Get an address without hitting the Internet.
            var address1 = BeeFrog.Iota.Api.Utils.IotaUtils.GenerateAddress(seed, 0);
            Console.WriteLine($"Your address is: {address1.Address}");

            var api = new IotaApi("https://nodes.thetangle.org/");

            // Get one address.
            Console.WriteLine($"Creating an address and checking it's balance.");
            var address1WithBalance = api.GetAddress(seed, 0).Result;
            Console.WriteLine($"Your address is: {address1WithBalance.Address} Your balance is: {address1WithBalance.Balance} Iota");

            // Get multiple address.
            Console.WriteLine($"Getting 3 addresses and checking their balance.");
            var addresses = api.GetAddresses(seed, 1, 3, CancellationToken.None).Result;
            foreach (var address in addresses)
            {
                Console.WriteLine($"Another address is: {address.Address} Your balance is: {address.Balance} Iota");
            }

            Console.WriteLine($"Press any key to close");
            Console.ReadKey();
        }
    }
}
