using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Iri.Dto
{
    /// <summary>
    /// This class represents the core API request 'GetTips'
    /// </summary>
    public class GetTipsRequest : IriRequestBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTipsRequest"/> class.
        /// </summary>
        public GetTipsRequest() : base(CommandConstants.GetTips)
        {
        }
    }
}
