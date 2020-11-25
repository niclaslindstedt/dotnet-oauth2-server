using System;

namespace Etimo.Id.Service.Exceptions
{
    public abstract class ErrorCodeException : Exception
    {
        public string ErrorCode { get; set; }

        protected ErrorCodeException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
