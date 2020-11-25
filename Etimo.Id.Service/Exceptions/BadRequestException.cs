namespace Etimo.Id.Service.Exceptions
{
    public class BadRequestException : ErrorCodeException
    {
        /// <summary>
        /// The user has made a request that the application cannot process.
        /// </summary>
        public BadRequestException(string message, string errorCode = "bad_request") : base(message, errorCode)
        {
        }
    }
}
