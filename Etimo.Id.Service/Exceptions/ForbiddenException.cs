using System;

namespace Etimo.Id.Service.Exceptions
{
    /// <summary>
    /// Used when an authenticated user is trying to access protected data that it does not have access to.
    /// </summary>
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "You are not authorized to view that.") : base(message)
        {
        }
    }
}
