namespace Etimo.Id.Service.Exceptions
{
    /// <summary>
    /// The authenticated client is not authorized to use this
    /// authorization grant type.
    /// </summary>
    public class UnauthorizedClientException : BadRequestException
    {
        public UnauthorizedClientException(string message) : base(message, "unauthorized_client")
        {
        }
    }
}
