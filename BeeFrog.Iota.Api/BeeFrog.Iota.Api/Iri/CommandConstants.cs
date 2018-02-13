﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Iri
{
    /// <summary>
    /// This enumeration defines the core API call commands
    /// </summary>
    public static class CommandConstants
    {
        /// <summary>
        /// Get information about the node.
        /// </summary>
        public const string GetNodeInfo = "getNodeInfo";

        /// <summary>
        /// Gets the tips of the node
        /// </summary>
        public const string GetTips = "getTips";

        /// <summary>
        /// Finds the transactions using different search criteria <see cref="FindTransactionsRequest"/>
        /// </summary>
        public const string FindTransactions = "findTransactions";

        /// <summary>
        /// Gets the transactions to approve
        /// </summary>
        public const string GetTransactionsToApprove = "getTransactionsToApprove";

        /// <summary>
        /// Attaches to the tangle
        /// </summary>
        public const string AttachToTangle = "attachToTangle";

        /// <summary>
        /// Gets the balances
        /// </summary>
        public const string GetBalances = "getBalances";

        /// <summary>
        /// Gets the inclusion state
        /// </summary>
        public const string GetInclusionStates = "getInclusionStates";

        /// <summary>
        /// Gets the trytes
        /// </summary>
        public const string GetTrytes = "getTrytes";

        /// <summary>
        /// Gets the neighbours of the node
        /// </summary>
        public const string GetNeighbors = "getNeighbors";

        /// <summary>
        /// Adds neighbours to the node
        /// </summary>
        public const string AddNeighbors = "addNeighbors";

        /// <summary>
        /// Removes neighbours from the node
        /// </summary>
        public const string RemoveNeighbors = "removeNeighbors";

        /// <summary>
        /// Interrupt attaching to the tangle
        /// </summary>
        public const string InterruptAttachingToTangle = "interruptAttachingToTangle";

        /// <summary>
        /// Broadcasts transactions
        /// </summary>
        public const string BroadcastTransactions = "broadcastTransactions";

        /// <summary>
        /// Stores transactions
        /// </summary>
        public const string StoreTransactions = "storeTransactions";

        /// <summary>
        /// Checks the consistency of the transaction to see if's been attached to a transaction which has been re-attached of is a double spend.
        /// </summary>
        public const string CheckConsistency = "checkConsistency";

        /// <summary>
        /// Check if a list of addresses was ever spent from, in the current epoch, or in previous epochs.
        /// </summary>
        public const string WereAddressesSpentFrom = "wereAddressesSpentFrom";
        
    }
}
