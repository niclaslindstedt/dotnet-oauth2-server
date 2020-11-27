namespace Etimo.Id.Service.Exceptions
{
    public class UnauthorizedException : ErrorCodeException
    {
        /// <summary>
        /// Used when trying to access protected data without being authenticated.
        /// </summary>
        public UnauthorizedException(string message = null, string errorCode = "unauthorized") : base(message, errorCode)
        {
        }
    }
}
