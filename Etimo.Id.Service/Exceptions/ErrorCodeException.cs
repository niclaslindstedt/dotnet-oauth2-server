using System;

namespace Etimo.Id.Service.Exceptions
{
    public class ErrorCodeException : Exception
    {
        public string ErrorCode { get; set; }

        public ErrorCodeException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
