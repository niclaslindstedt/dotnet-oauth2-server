using System;
using System.Collections.Generic;

namespace Etimo.Id.Exceptions
{
    public abstract class ErrorCodeException : Exception
    {
        protected ErrorCodeException(
            string message,
            string errorCode,
            Exception exception)
            : base(message, exception)
        {
            ErrorCode = errorCode;
        }

        protected ErrorCodeException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public string                     ErrorCode { get; set; }
        public Dictionary<string, string> Headers   { get; set; } = new();
        public string                     State     { get; set; }
    }
}
