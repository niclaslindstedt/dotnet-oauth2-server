namespace Etimo.Id.Service.Exceptions
{
    public class InvalidRequestException : BadRequestException
    {
        /// <summary>
        /// The request is missing a required parameter, includes an unsupported parameter value (other than grant type), repeats a parameter, includes multiple credentials, utilizes more than one mechanism for authenticating the client, or is otherwise malformed.
        /// Read more: https://tools.ietf.org/html/rfc6749#section-5.2
        /// </summary>
        public InvalidRequestException(string message)
            : base(message, "invalid_request")
        {
        }

        /// <summary>
        /// The request is missing a required parameter, includes an invalid parameter value, includes a parameter more than once, or is otherwise malformed.
        /// Read more: https://tools.ietf.org/html/rfc6749#section-4.1.2.1
        /// </summary>
        public InvalidRequestException(string message, string state)
            : base(message, "invalid_request")
        {
            State = state;
        }
    }
}
