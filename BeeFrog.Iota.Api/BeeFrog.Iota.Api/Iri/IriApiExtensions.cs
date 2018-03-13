using BeeFrog.Iota.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeeFrog.Iota.Api.Iri
{
    public static class IriApiExtensions
    {
        public static async Task<APIResult<string[]>> FindTransactionsFromAddresses(this IriApi api, params string[] addresses)
        {
            var transactionTrytes = await api.FindTransactions(addresses, null, null, null);
            return transactionTrytes;
        }

        public static async Task<APIResult<TransactionItem[]>> FindTransactionItemsFromAddresses(this IriApi api, params string[] addresses)
        {
            var transactionHashes = await api.FindTransactionsFromAddresses(addresses);
            if (transactionHashes.Successful)
            {
                return await api.FindTransactionItemsFromHashes(transactionHashes.Result);
            }
            return transactionHashes.RePackage(r => new TransactionItem[0]);
        }

        public static async Task<APIResult<TransactionItem[]>> FindTransactionItemsFromHashes(this IriApi api, params string[] hashes)
        {
            var transactionTrytes = await api.GetTrytes(hashes);
            if (transactionTrytes.Successful)
            {
                var trans = transactionTrytes.Result.Select(t => new TransactionItem(t)).ToArray();
                return new APIResult<TransactionItem[]>(trans);
            }

            return transactionTrytes.RePackage(r => new TransactionItem[0]);
        }

        /// <summary>
        /// Renew address items balances
        /// </summary>
        /// <param name="api">Iri api</param>
        /// <param name="addressItems">Address items to renew</param>
        /// <returns></returns>
        public static async Task RenewBalances(this IriApi api, params AddressItem[] addressItems)
        {
            if (addressItems.Length == 0) return;
            var addresses = addressItems.Select(a => a.Address).ToArray();
            var balances = await api.GetBalances(addresses);
            if (balances.Successful)
            {
                for (int i = 0; i < addressItems.Length; i++)
                {
                    addressItems[i].Balance = balances.Result[i];
                }
            }
        }

        public static Task<APIResult<long[]>> GetBalances(this IriApi api, params string[] addresses)
        {
            return api.GetBalances(addresses, 100);
        }
    }
}
