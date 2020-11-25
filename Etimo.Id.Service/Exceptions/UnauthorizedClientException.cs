namespace Etimo.Id.Service.Exceptions
{
    public class UnauthorizedClientException : BadRequestException
    {
        /// <summary>
        /// The authenticated client is not authorized to use this
        /// authorization grant type.
        /// </summary>
        public UnauthorizedClientException(string message) : base(message, "unauthorized_client")
        {
        }
    }
}
