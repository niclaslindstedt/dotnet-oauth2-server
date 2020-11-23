using System;

namespace Etimo.Id.Service.Exceptions
{
    public class NotFoundException : ErrorCodeException
    {
        public NotFoundException(string message = null) : base(message, "not_found")
        {
        }
    }
}
