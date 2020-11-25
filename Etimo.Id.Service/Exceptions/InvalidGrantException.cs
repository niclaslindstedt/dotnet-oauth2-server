namespace Etimo.Id.Service.Exceptions
{
    public class InvalidGrantException : BadRequestException
    {
        /// <summary>
        /// The provided authorization grant (e.g., authorization
        /// code, resource owner credentials) or refresh token is
        /// invalid, expired, revoked, does not match the redirection
        /// URI used in the authorization request, or was issued to
        /// another client.
        /// </summary>
        public InvalidGrantException(string message) : base(message, "invalid_grant")
        {
        }
    }
}
