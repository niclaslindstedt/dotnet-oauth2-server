using System;

namespace Etimo.Id.Service.Exceptions
{
    public class InvalidGrantException : Exception
    {
        public InvalidGrantException() : base("invalid_grant")
        {
        }
    }
}
