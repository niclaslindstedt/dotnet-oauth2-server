namespace Etimo.Id.Exceptions
{
    public class TemporarilyUnavailableException : BadRequestException
    {
        /// <summary>
        ///     The authorization server is currently unable to handle the request due to a temporary overloading or maintenance of
        ///     the server.
        ///     Read more: https://tools.ietf.org/html/rfc6749#section-4.1.2.1
        /// </summary>
        public TemporarilyUnavailableException(string message, string state = null)
            : base(message, "temporarily_unavailable")
        {
            State = state;
        }
    }
}
