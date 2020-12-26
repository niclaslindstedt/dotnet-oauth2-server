using System;
using System.Collections.Generic;

namespace Etimo.Id.Service.Exceptions
{
    public abstract class ErrorCodeException : Exception
    {
        public string ErrorCode { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string State { get; set; }

        protected ErrorCodeException(string message, string errorCode, Exception exception)
            : base(message, exception)
        {
            ErrorCode = errorCode;
        }

        protected ErrorCodeException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
