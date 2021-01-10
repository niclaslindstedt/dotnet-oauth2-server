namespace Etimo.Id.Exceptions
{
    public class UnauthorizedException : ErrorCodeException
    {
        /// <summary>
        ///     Used when trying to access protected data without being authenticated.
        /// </summary>
        public UnauthorizedException(string message = "You have to authenticate to do that.", string errorCode = "unauthorized")
            : base(message, errorCode) { }
    }
}
