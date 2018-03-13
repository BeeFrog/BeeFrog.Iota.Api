using System;
using BeeFrog.Iota.Api;

namespace CheckAnAddressHAsBeenUsed
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new IotaApi("https://nodes.thetangle.org/");

            // Get one address.
            Console.WriteLine($"Checking if addresses have been used before.");
            var addresses = new[]{
                "FAKEADDRESS9999999999999999999999999999999999999999999999999999999999999999999999",
                "ALVYY9WDRDISRKUVSVGD9LVLDPBVETCSJQBXKFC9FNBYPVDEUSOCIWMVAESUOPCAAJTMXNPKHHIYTOH9X" };

            var response = api.IriApi.WereAddressesSpentFrom(addresses).Result;

            if (response.Successful)
            {
                var data = response.Result;
                for (int i = 0; i < addresses.Length; i++)
                {
                    Console.WriteLine($"Address {addresses[i]} used={data[i]}");
                }
            }
            else
            {
                Console.WriteLine($"{response.ErrorMessage} Exception: {response.ExceptionInfo}");
            }

            Console.WriteLine($"Press any key to close");
            Console.ReadKey();
        }
    }
}
