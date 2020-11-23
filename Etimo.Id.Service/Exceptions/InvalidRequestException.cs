using System;

namespace Etimo.Id.Service.Exceptions
{
    public class InvalidRequestException : ErrorCodeException
    {
        public InvalidRequestException() : base("invalid_request", "invalid_request")
        {
        }
    }
}
