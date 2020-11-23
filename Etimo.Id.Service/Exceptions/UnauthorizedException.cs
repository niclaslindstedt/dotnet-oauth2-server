using System;

namespace Etimo.Id.Service.Exceptions
{
    /// <summary>
    /// Used when trying to access protected data without being authenticated.
    /// </summary>
    public class UnauthorizedException : ErrorCodeException
    {
        public UnauthorizedException(string message = null) : base(message, "unauthorized")
        {
        }
    }
}
