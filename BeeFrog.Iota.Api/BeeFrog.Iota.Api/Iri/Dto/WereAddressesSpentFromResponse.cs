using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Iri.Dto
{
    /// <summary>
    /// This class represents the response of <see cref="WereAddressSpentFromRequest"/>
    /// </summary>
    /// <seealso cref="BeeFrog.Iota.Api.Iri.Dto.IriResponseBase" />
    public class WereAddressesSpentFromResponse : IriResponseBase
    {
        /// <summary>
        /// Gets or sets the states.
        /// </summary>
        /// <value>
        /// The states, true if the address has been used otherwise false.
        /// </value>
        public bool[] States { get; set; }
    }
}
