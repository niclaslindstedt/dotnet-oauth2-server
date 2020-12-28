namespace Etimo.Id.Service.Exceptions
{
    public class UnsupportedResponseTypeException : BadRequestException
    {
        /// <summary>
        ///     The authorization server does not support obtaining an access token using this method.
        ///     Read more: https://tools.ietf.org/html/rfc6749#section-4.1.2.1
        /// </summary>
        public UnsupportedResponseTypeException(string message, string state = null)
            : base(message, "unsupported_response_type")
        {
            State = state;
        }
    }
}
