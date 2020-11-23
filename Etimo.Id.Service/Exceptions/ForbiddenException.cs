using System;

namespace Etimo.Id.Service.Exceptions
{
    /// <summary>
    /// Used when an authenticated user is trying to access protected data that it does not have access to.
    /// </summary>
    public class ForbiddenException : ErrorCodeException
    {
        public ForbiddenException(string message = null) : base(message, "forbidden")
        {
        }
    }
}
