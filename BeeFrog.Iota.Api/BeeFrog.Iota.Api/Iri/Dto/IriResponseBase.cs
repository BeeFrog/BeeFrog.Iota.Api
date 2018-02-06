using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Iri.Dto
{
    /// <summary>
    /// This class represents the base class of different core API response classes
    /// </summary>
    public class IriResponseBase
    {
        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public long Duration { get; set; }
    }
}
