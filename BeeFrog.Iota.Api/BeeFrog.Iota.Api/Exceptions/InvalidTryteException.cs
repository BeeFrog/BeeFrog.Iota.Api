using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Exceptions
{
    public class InvalidTryteException : IotaException
    {
        public InvalidTryteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidTryteException(string message)
            : base(message)
        {
        }

        public InvalidTryteException()
            : base("The specified tryte is invalid")
        {

        }
    }
}
