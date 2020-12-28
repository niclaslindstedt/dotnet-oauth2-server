namespace Etimo.Id.Service.Exceptions
{
    public class InvalidClientException : UnauthorizedException
    {
        /// <summary>
        ///     Client authentication failed (e.g., unknown client, no client authentication included, or unsupported
        ///     authentication method).
        ///     Read more: https://tools.ietf.org/html/rfc6749#section-5.2
        /// </summary>
        public InvalidClientException(string message, string state = null)
            : base(message, "invalid_client")
        {
            State = state;

            // https://tools.ietf.org/html/rfc6749#section-5.2
            Headers.Add("WWW-Authenticate", $"Bearer error=\"invalid_client\" error_description=\"{message}\"");
        }
    }
}
