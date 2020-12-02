namespace Etimo.Id.Service.Exceptions
{
    public class InvalidScopeException : BadRequestException
    {
        /// <summary>
        /// The requested scope is invalid, unknown, malformed, or exceeds the scope granted by the resource owner.
        /// Read more: https://tools.ietf.org/html/rfc6749#section-5.2
        /// </summary>
        public InvalidScopeException(string message)
            : base(message, "invalid_scope")
        {
        }

        /// <summary>
        /// The requested scope is invalid, unknown, or malformed.
        /// Read more: https://tools.ietf.org/html/rfc6749#section-4.1.2.1
        /// </summary>
        public InvalidScopeException(string message, string state)
            : base(message, "invalid_scope")
        {
            State = state;
        }
    }
}
