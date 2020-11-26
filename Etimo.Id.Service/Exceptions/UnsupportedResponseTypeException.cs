namespace Etimo.Id.Service.Exceptions
{
    public class UnsupportedResponseTypeException : BadRequestException
    {
        /// <summary>
        /// The authorization server does not support obtaining an
        /// access token using this method.
        /// </summary>
        public UnsupportedResponseTypeException(string message) : base(message, "unsupported_response_type")
        {
        }
    }
}
