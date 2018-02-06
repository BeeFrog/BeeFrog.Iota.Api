using System.Collections.Generic;
using BeeFrog.Iota.Api.Models;

namespace BeeFrog.Iota.Api.Iri.Dto
{
    /// <summary>
    /// Response of <see cref="GetNeighborsRequest"/>
    /// </summary>
    public class GetNeighborsResponse
    {
        /// <summary>
        /// Gets or sets the neighbors.
        /// </summary>
        /// <value>
        /// The neighbors.
        /// </value>
        public Neighbor[] Neighbors { get; set; }
    }
}