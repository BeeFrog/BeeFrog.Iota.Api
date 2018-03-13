using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFrog.Iota.Api.Iri
{
    public class APIResult<TResult>
    {
        /// <summary>
        /// Creates a successful Result.
        /// </summary>
        /// <param name="result"></param>
        public APIResult(TResult result)
        {
            this.Successful = true;
            this.Result = result;
            this.ExceptionInfo = null;
        }

        /// <summary>
        /// Creates a failed API result with the reasons.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="errorMessage"></param>
        public APIResult(Exception exception, string errorMessage)
        {
            this.Successful = false;
            this.ExceptionInfo = exception;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// The result from the successful API call.
        /// </summary>
        public TResult Result { get; private set; }

        /// <summary>
        /// True, if the request is successful, otherwise false.
        /// </summary>
        public bool Successful { get; private set; }

        /// <summary>
        /// The presentable error message when not successful.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// The exception (if one exists)
        /// </summary>
        public Exception ExceptionInfo { get; private set; }

        public APIResult<T> RePackage<T>(Func<TResult,T> func)
        {
            if(this.Successful)
            {
                return new APIResult<T>(func(this.Result));
            }
            else
            {
                return new APIResult<T>(this.ExceptionInfo, this.ErrorMessage);
            }
        }
    }
}
