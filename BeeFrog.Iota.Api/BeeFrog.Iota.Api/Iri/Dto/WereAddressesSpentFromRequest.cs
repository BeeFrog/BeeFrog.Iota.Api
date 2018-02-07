using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Iri.Dto
{
    /// <summary>
    /// Checks if any address have been used before to send funds.
    /// If so you should not use them again as their security is now compromised.
    /// </summary>
    public class WereAddressesSpentFromRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WereAddressesSpentFromRequest"/> class.
        /// </summary>
        /// <param name="addresses">The addresses (Without checksum) to check.</param>
        public WereAddressesSpentFromRequest(string[] addresses) : base(CommandConstants.WereAddressesSpentFrom)
        {
            this.Addresses = addresses;
        }
        
        /// <summary>
        /// An array of address to check (without checksum).
        /// </summary>
        public string[] Addresses { get; }
    }
}
