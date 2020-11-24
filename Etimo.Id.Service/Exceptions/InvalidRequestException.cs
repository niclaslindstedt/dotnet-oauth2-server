using System;

namespace Etimo.Id.Service.Exceptions
{
    public class InvalidRequestException : BadRequestException
    {
        public InvalidRequestException(string message) : base(message, "invalid_request")
        {
        }
    }
}
