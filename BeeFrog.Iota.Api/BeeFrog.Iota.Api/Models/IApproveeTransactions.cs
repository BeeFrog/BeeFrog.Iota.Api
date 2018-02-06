using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Models
{
    public interface IApproveTransactions
    {
        /// <summary>
        /// Gets or sets the trunk transaction.
        /// </summary>
        /// <value>
        /// The trunk transaction.
        /// </value>
        string TrunkTransaction { get; }

        /// <summary>
        /// Gets or sets the branch transaction.
        /// </summary>
        /// <value>
        /// The branch transaction.
        /// </value>
        string BranchTransaction { get; }
    }
}
