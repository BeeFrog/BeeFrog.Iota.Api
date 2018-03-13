using BeeFrog.Iota.Api.Iri.Dto;
using BeeFrog.Iota.Api.Models;
using BeeFrog.Iota.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BeeFrog.Iota.Api.Iri
{
    public class IriApi : INonceSeeker
    {
        private readonly IGenericWebClient genericWebClient;

        /// <summary>
        /// Gets or sets the MinWeightMagnitude. Default is 15
        /// </summary>
        public int MinWeightMagnitude { get; set; }

        /// <summary>
        /// Gets or sets the TransactionApproveDepth. Default is 9
        /// </summary>
        public int TransactionApproveDepth { get; set; }

        /// <summary>
        /// Gets the WebClient
        /// </summary>
        public IGenericWebClient WebClient => genericWebClient;

        public IriApi(string url) : this(new GenericWebClient(url))
        {
        }

        public IriApi(IGenericWebClient genericClient2)
        {            
            this.genericWebClient = genericClient2 ?? throw new ArgumentNullException(nameof(genericClient2));

            this.MinWeightMagnitude = 15;
            this.TransactionApproveDepth = 9;
        }

        public async Task<APIResult<string[]>> AttachToTangle(string[] trytes, CancellationToken cancellationToken)
        {
            var transactionsToApprove = await GetTransactionsToApprove(TransactionApproveDepth);
            if (transactionsToApprove.Successful)
            {
                var result = await AttachToTangle(trytes,
                    transactionsToApprove.Result.TrunkTransaction, 
                    transactionsToApprove.Result.BranchTransaction,
                    cancellationToken);

                return result;
            }
            else
            {
                return transactionsToApprove.RePackage(r => new string[0]);
            }
        }

        public async Task<APIResult<string[]>> AttachToTangle(string[] trytes,
            string trunkTransaction, string branchTransaction, CancellationToken cancellationToken)
        {
            InputValidator.CheckIfArrayOfTrytes(trytes);

            AttachToTangleRequest attachToTangleRequest = new AttachToTangleRequest(trunkTransaction, branchTransaction, trytes, MinWeightMagnitude);

            var response = await genericWebClient.RequestAsync<AttachToTangleResponse>(attachToTangleRequest, cancellationToken);

            return response?.RePackage(r => r.Trytes) ?? new APIResult<string[]>(null, "Null response from API");
        }

        public async Task BroadcastTransactions(params string[] trytes)
        {
            await genericWebClient.RequestAsync<BroadcastTransactionsResponse>(new BroadcastTransactionsRequest(trytes));
        }

        public async Task<APIResult<string[]>> FindTransactions(IEnumerable<string> addresses, IEnumerable<string> tags,
            IEnumerable<string> approves, IEnumerable<string> bundles)
        {
            FindTransactionsRequest findTransactionsRequest =
                new FindTransactionsRequest(bundles?.ToArray(), addresses?.ToArray(), tags?.ToArray(), approves?.ToArray());

            var response = await genericWebClient.RequestAsync<FindTransactionsResponse>(findTransactionsRequest);

            return response?.RePackage(r => r.Hashes);
        }

        public async Task<APIResult<long[]>> GetBalances(IEnumerable<string> addresses, long threshold)
        {
            GetBalancesRequest getBalancesRequest = new GetBalancesRequest(addresses.ToArray(), threshold);            
            var response = await genericWebClient.RequestAsync<GetBalancesResponse>(getBalancesRequest);

            return response?.RePackage(r => r.Balances);
        }

        public async Task<APIResult<bool[]>> GetInclusionStates(IEnumerable<string> transactions, params string[] milestones)
        {
            var response = await genericWebClient.RequestAsync<GetInclusionStatesResponse>(new GetInclusionStatesRequest(transactions.ToArray(), milestones));

            return response?.RePackage(r => r.States);
        }

        public async Task StoreTransactions(params string[] trytes)
        {
            var response = await genericWebClient.RequestAsync<StoreTransactionsResponse>(new StoreTransactionsRequest(trytes));
        }

        public async Task<APIResult<GetNodeInfoResponse>> GetNodeInfo()
        {
            var response = await genericWebClient.RequestAsync<GetNodeInfoResponse>(new GetNodeInfoRequest());            
            return response;
        }

        public async Task<APIResult<string[]>> GetTips()
        {
            GetTipsRequest getTipsRequest = new GetTipsRequest();
            var response = await genericWebClient.RequestAsync<GetTipsResponse>(getTipsRequest);

            return response?.RePackage(r => r.Hashes);
        }

        public async Task<APIResult<IApproveTransactions>> GetTransactionsToApprove(int depth)
        {
            GetTransactionsToApproveRequest getTransactionsToApproveRequest = new GetTransactionsToApproveRequest(depth);
            var response = await genericWebClient.RequestAsync<GetTransactionsToApproveResponse>(getTransactionsToApproveRequest, CancellationToken.None);

            var result = response ?? this.NullResponse<GetTransactionsToApproveResponse>();
            return result.RePackage(r => r as IApproveTransactions);
        }

        public async Task<APIResult<string[]>> GetTrytes(params string[] hashes)
        {
            GetTrytesRequest getTrytesRequest = new GetTrytesRequest() { Hashes = hashes };
            var response = await genericWebClient.RequestAsync<GetTrytesResponse>(getTrytesRequest);

            return response?.RePackage(r => r.Trytes);
        }

        public async Task<bool> InterruptAttachingToTangle()
        {
            InterruptAttachingToTangleRequest request = new InterruptAttachingToTangleRequest();
            var response = await genericWebClient.RequestAsync<InterruptAttachingToTangleResponse>(request);

            return response?.Successful == true;
        }

        public async Task<APIResult<Neighbor[]>> GetNeighbors()
        {
            GetNeighborsRequest getNeighborsRequest = new GetNeighborsRequest();
            var response = await genericWebClient.RequestAsync<GetNeighborsResponse>(getNeighborsRequest);

            return response?.RePackage(r => r.Neighbors);
        }

        public async Task<APIResult<long>> AddNeighbors(params string[] uris)
        {
            var response = await genericWebClient.RequestAsync<AddNeighborsResponse>(new AddNeighborsRequest(uris));

            return response?.RePackage(r => r.AddedNeighbors);
        }

        public async Task<APIResult<long>> RemoveNeighbors(params string[] uris)
        {
            RemoveNeighborsRequest removeNeighborsRequest = new RemoveNeighborsRequest(uris);
            var response = await genericWebClient.RequestAsync<RemoveNeighborsResponse>(removeNeighborsRequest);

            return response?.RePackage(r => r.RemovedNeighbors);
        }

        /// <summary>
        /// Checks the transaction hashes are attached to a valid transactions which will be confirmed.
        /// If you have attached to a double spend transaction of a transaction which has been re-attached it will be classed as inconsistent.
        /// </summary>
        /// <param name="hashes">The transaction hashes to check.</param>
        /// <returns>An array of booleans in the same order as the hashes</returns>
        public async Task<APIResult<bool>> CheckConsistency(params string[] hashes)
        {
            var request = new CheckConsistencyRequest(hashes);
            var response = await genericWebClient.RequestAsync<CheckConsistencyResponse>(request);
            
            return response?.RePackage(r => r.State);
        }

        /// <summary>
        /// Checks if any of the addresses have been used before to send Iota.        
        /// </summary>
        /// <param name="addresses">the address (without checksum) to check.</param>
        /// <returns>An array of boolean in the same order as the addresses. True means the address has been used to send Iota.</returns>
        public async Task<APIResult<bool[]>> WereAddressesSpentFrom(params string[] addresses)
        {
            var request = new WereAddressesSpentFromRequest(addresses);
            var response = await genericWebClient.RequestAsync<WereAddressesSpentFromResponse>(request);

            return response?.RePackage(r => r.States)?? this.NullResponse<bool[]>();
        }

        async Task<string[]> INonceSeeker.SearchNonce(string[] trytes, string trunkTransaction, string branchTransaction, CancellationToken cancellationToken)
        {
            var result = await AttachToTangle(trytes, trunkTransaction, branchTransaction, cancellationToken);
            return result?.Result;
        }

        private APIResult<T> NullResponse<T>()
        {
            return new APIResult<T>(null, "Null response from API");
        }
    }
}
