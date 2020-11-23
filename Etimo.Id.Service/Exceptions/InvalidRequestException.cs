using System;

namespace Etimo.Id.Service.Exceptions
{
    public class InvalidRequestException : Exception
    {
        public InvalidRequestException() : base("invalid_request")
        {
        }
    }
}
