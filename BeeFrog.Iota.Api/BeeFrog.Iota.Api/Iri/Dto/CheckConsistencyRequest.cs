using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Iri.Dto
{
    public class CheckConsistencyRequest : IriRequestBase
    {
        public CheckConsistencyRequest(string[] transactionHashs) : base(CommandConstants.CheckConsistency)
        {
            this.Tails = transactionHashs;
        }

        public string[] Tails { get; }
    }
}
