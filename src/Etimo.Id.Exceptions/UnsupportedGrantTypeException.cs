namespace Etimo.Id.Exceptions
{
    public class UnsupportedGrantTypeException : BadRequestException
    {
        /// <summary>
        ///     The authorization grant type is not supported by the authorization server.
        ///     Read more: https://tools.ietf.org/html/rfc6749#section-5.2
        /// </summary>
        public UnsupportedGrantTypeException(string message)
            : base(message, "unsupported_grant_type") { }
    }
}
