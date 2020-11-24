using System;

namespace Etimo.Id.Service.Exceptions
{
    public class BadRequestException : ErrorCodeException
    {
        public BadRequestException(string message, string errorCode = "bad_request") : base(message, errorCode)
        {
        }
    }
}
