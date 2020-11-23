using System;

namespace Etimo.Id.Service.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message = null) : base(message)
        {
        }
    }
}
