using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Iri.Dto
{
    public class CheckConsistencyResponse
    {
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state, true if the hashes are consistent and can be promoted otherwise false.
        /// </value>
        public bool State { get; set; }

        /// <summary>
        /// Any extra info from the IRI, probably likely to be null unless your transaction cannot be promoted.
        /// </summary>
        public string Info { get; set; }
    }
}
