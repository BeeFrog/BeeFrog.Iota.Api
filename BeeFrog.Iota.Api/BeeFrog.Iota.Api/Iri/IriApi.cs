﻿using BeeFrog.Iota.Api.Iri.Dto;
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
        private readonly IGenericWebClient genericClient;

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
        public IGenericWebClient WebClient => genericClient;

        public IriApi(string url)
            : this(new GenericWebClient(url))
        {
        }

        public IriApi(IGenericWebClient genericClient)
        {
            if (genericClient == null)
                throw new ArgumentNullException(nameof(genericClient));

            this.genericClient = genericClient;
            this.MinWeightMagnitude = 15;
            this.TransactionApproveDepth = 9;
        }

        public async Task<string[]> AttachToTangle(string[] trytes, CancellationToken cancellationToken)
        {
            var transactionsToApprove = await GetTransactionsToApprove(TransactionApproveDepth);
            return await AttachToTangle(trytes,
                transactionsToApprove.TrunkTransaction, transactionsToApprove.BranchTransaction, 
                cancellationToken);
        }

        public async Task<string[]> AttachToTangle(string[] trytes, 
            string trunkTransaction, string branchTransaction, CancellationToken cancellationToken)
        {
            InputValidator.CheckIfArrayOfTrytes(trytes);

            AttachToTangleRequest attachToTangleRequest = new AttachToTangleRequest(
                trunkTransaction, branchTransaction,
                trytes, MinWeightMagnitude);
            var response = await genericClient.RequestAsync<AttachToTangleResponse>(attachToTangleRequest, cancellationToken);
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.Trytes;
        }

        public async Task BroadcastTransactions(params string[] trytes)
        {
            await genericClient.RequestAsync<BroadcastTransactionsResponse>(
                    new BroadcastTransactionsRequest(trytes));
        }

        public async Task<string[]> FindTransactions(IEnumerable<string> addresses, IEnumerable<string> tags,
            IEnumerable<string> approves, IEnumerable<string> bundles)
        {
            FindTransactionsRequest findTransactionsRequest = 
                new FindTransactionsRequest(bundles?.ToArray(), addresses?.ToArray(), tags?.ToArray(), approves?.ToArray());

            var response = await genericClient.RequestAsync<FindTransactionsResponse>(findTransactionsRequest);
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.Hashes;
        }

        public async Task<long[]> GetBalances(IEnumerable<string> addresses, long threshold)
        {
            GetBalancesRequest getBalancesRequest = new GetBalancesRequest(addresses.ToArray(), threshold);
            var response = await genericClient.RequestAsync<GetBalancesResponse>(getBalancesRequest);
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.Balances;
        }

        public async Task<bool[]> GetInclusionStates(IEnumerable<string> transactions, params string[] milestones)
        {
            var response = await genericClient.RequestAsync<GetInclusionStatesResponse>(
                    new GetInclusionStatesRequest(transactions.ToArray(), milestones));
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.States;
        }

        public async Task StoreTransactions(params string[] trytes)
        {
            var response = await genericClient.RequestAsync<StoreTransactionsResponse>(
                    new StoreTransactionsRequest(trytes));
        }

        public async Task<GetNodeInfoResponse> GetNodeInfo()
        {
            var response = await genericClient.RequestAsync<GetNodeInfoResponse>(new GetNodeInfoRequest());
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response;
        }

        public async Task<string[]> GetTips()
        {
            GetTipsRequest getTipsRequest = new GetTipsRequest();
            var response = await genericClient.RequestAsync<GetTipsResponse>(getTipsRequest);
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.Hashes;
        }

        public async Task<IApproveTransactions> GetTransactionsToApprove(int depth)
        {
            GetTransactionsToApproveRequest getTransactionsToApproveRequest = new GetTransactionsToApproveRequest(depth);
            var response = await genericClient.RequestAsync<GetTransactionsToApproveResponse>(
                    getTransactionsToApproveRequest);
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response;
        }

        public async Task<string[]> GetTrytes(params string[] hashes)
        {
            GetTrytesRequest getTrytesRequest = new GetTrytesRequest() { Hashes = hashes };
            var response = await genericClient.RequestAsync<GetTrytesResponse>(getTrytesRequest);
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.Trytes;
        }

        public async Task InterruptAttachingToTangle()
        {
            InterruptAttachingToTangleRequest request = new InterruptAttachingToTangleRequest();
            var response = await genericClient.RequestAsync<InterruptAttachingToTangleResponse>(request);
        }

        public async Task<Neighbor[]> GetNeighbors()
        {
            GetNeighborsRequest getNeighborsRequest = new GetNeighborsRequest();
            var response = await genericClient.RequestAsync<GetNeighborsResponse>(getNeighborsRequest);
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.Neighbors;
        }

        public async Task<long> AddNeighbors(params string[] uris)
        {
            var response = await genericClient.RequestAsync<AddNeighborsResponse>(
                    new AddNeighborsRequest(uris));
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.AddedNeighbors;
        }

        public async Task<long> RemoveNeighbors(params string[] uris)
        {
            RemoveNeighborsRequest removeNeighborsRequest = new RemoveNeighborsRequest(uris);
            var response = await genericClient.RequestAsync<RemoveNeighborsResponse>(removeNeighborsRequest);
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.RemovedNeighbors;
        }

        /// <summary>
        /// Checks the transaction hashes are attached to a valid transactions which will be confirmed.
        /// If you have attached to a double spend transaction of a transaction which has been re-attached it will be classed as inconsistent.
        /// </summary>
        /// <param name="hashes">The transaction hashes to check.</param>
        /// <returns>An array of booleans in the same order as the hashes</returns>
        public async Task<bool> CheckConsistency(params string[] hashes)
        {
            var request = new CheckConsistencyRequest(hashes);
            var response = await genericClient.RequestAsync<CheckConsistencyResponse>(request);
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.State;
        }

        /// <summary>
        /// Checks if any of the addresses have been used before to send Iota.        
        /// </summary>
        /// <param name="addresses">the address (without checksum) to check.</param>
        /// <returns>An array of booleans in the same order as the addresses. True means the address has been used to send Iota.</returns>
        public async Task<bool[]> WereAddressesSpentFrom(params string[] addresses)
        {
            var request = new WereAddressesSpentFromRequest(addresses);
            var response = await genericClient.RequestAsync<WereAddressesSpentFromResponse>(request);
            if (response == null)
                throw new NullReferenceException(nameof(response));

            return response.States;
        }

        async Task<string[]> INonceSeeker.SearchNonce(string[] trytes, string trunkTransaction, string branchTransaction, CancellationToken cancellationToken)
        {
            return await AttachToTangle(trytes, trunkTransaction, branchTransaction, cancellationToken);
        }

    }
}
