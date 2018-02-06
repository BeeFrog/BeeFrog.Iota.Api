﻿using System.Collections.Generic;

namespace BeeFrog.Iota.Api.Iri.Dto
{
    /// <summary>
    /// Response of <see cref="AttachToTangleRequest"/>
    /// </summary>
    public class AttachToTangleResponse
    {
        /// <summary>
        /// Gets or sets the trytes.
        /// </summary>
        /// <value>
        /// The trytes.
        /// </value>
        public string[] Trytes { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{nameof(Trytes)}: {string.Join(",", Trytes)}";
        }
    }
}