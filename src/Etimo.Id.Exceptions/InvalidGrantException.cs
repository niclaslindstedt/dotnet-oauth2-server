namespace Etimo.Id.Exceptions
{
    public class InvalidGrantException : BadRequestException
    {
        /// <summary>
        ///     The provided authorization grant (e.g., authorization code, resource owner credentials) or refresh token is
        ///     invalid, expired, revoked, does not match the redirection URI used in the authorization request, or was issued to
        ///     another client.
        ///     Read more: https://tools.ietf.org/html/rfc6749#section-5.2
        /// </summary>
        public InvalidGrantException(string message, string state = null)
            : base(message, "invalid_grant")
        {
            State = state;
        }
    }
}
