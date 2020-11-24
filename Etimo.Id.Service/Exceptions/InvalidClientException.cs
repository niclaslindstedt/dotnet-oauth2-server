namespace Etimo.Id.Service.Exceptions
{
    /// <summary>
    /// Client authentication failed (e.g., unknown client, no
    /// client authentication included, or unsupported
    /// authentication method).
    /// </summary>
    public class InvalidClientException : BadRequestException
    {
        public InvalidClientException(string message) : base(message, "invalid_client")
        {
        }
    }
}
