using System;

namespace Etimo.Id.Service.Exceptions
{
    public class ConflictException : ErrorCodeException
    {
        public ConflictException(string message) : base(message, "conflict")
        {
        }
    }
}
