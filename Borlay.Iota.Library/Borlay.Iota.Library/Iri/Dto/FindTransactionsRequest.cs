using System.Collections.Generic;

namespace Borlay.Iota.Library.Iri.Dto
{
    /// <summary>
    /// This class represents the core api request 'FindTransactions'
    /// </summary>
    public class FindTransactionsRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindTransactionsRequest"/> class.
        /// </summary>
        /// <param name="bundles">The bundles.</param>
        /// <param name="addresses">The addresses.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="approvees">The approvees.</param>
        public FindTransactionsRequest(string[] bundles, string[] addresses, string[] tags,
            string[] approvees) : base(CommandConstants.FindTransactions)
        {
            this.Bundles = bundles;
            this.Addresses = addresses;
            this.Tags = tags;
            this.Approvees = approvees;
        }

        /// <summary>
        /// Gets or sets the addresses.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        public string[] Addresses { get; set; }

        /// <summary>
        /// Gets or sets the bundles.
        /// </summary>
        /// <value>
        /// The bundles.
        /// </value>
        public string[] Bundles { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public string[] Tags { get; set; }

        /// <summary>
        /// Gets or sets the approvees.
        /// </summary>
        /// <value>
        /// The approvees.
        /// </value>
        public string[] Approvees { get; set; }
    }
}