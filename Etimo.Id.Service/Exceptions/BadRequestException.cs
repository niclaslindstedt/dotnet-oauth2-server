using System;

namespace Etimo.Id.Service.Exceptions
{
    public class BadRequestException : ErrorCodeException
    {
        public BadRequestException(string message) : base(message, "bad_request")
        {
        }
    }
}
