using System;
using System.Collections.Generic;

namespace Etimo.Id.Service.Exceptions
{
    public abstract class ErrorCodeException : Exception
    {
        public string ErrorCode { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        protected ErrorCodeException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
