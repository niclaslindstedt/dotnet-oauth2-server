namespace Etimo.Id.Service.Exceptions
{
    public class TooManyRequestsException : ErrorCodeException
    {
        /// <summary>
        ///     The user has made too many unauthorized/forbidden requests.
        /// </summary>
        public TooManyRequestsException(string message, int retryAfterSeconds = 60)
            : base(message, "too_many_requests")
        {
            Headers.Add("Retry-After", retryAfterSeconds.ToString());
        }
    }
}
