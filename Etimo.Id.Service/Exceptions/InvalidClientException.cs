namespace Etimo.Id.Service.Exceptions
{
    public class InvalidClientException : UnauthorizedException
    {
        /// <summary>
        /// Client authentication failed (e.g., unknown client, no
        /// client authentication included, or unsupported
        /// authentication method).
        /// </summary>
        public InvalidClientException(string message) : base(message, "invalid_client")
        {
        }
    }
}
