using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Exceptions
{
    /// <summary>
    /// This exception occurs when an illegal state is encountered
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class IllegalStateException : IotaException
    {
        public IllegalStateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public IllegalStateException(string message)
            : base(message)
        {
        }
    }
}
