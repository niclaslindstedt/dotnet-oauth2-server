namespace Etimo.Id.Service.Exceptions
{
    /// <summary>
    /// The authorization grant type is not supported by the
    /// authorization server.
    /// </summary>
    public class UnsupportedGrantTypeException : BadRequestException
    {
        public UnsupportedGrantTypeException(string message) : base(message, "unsupported_grant_type")
        {
        }
    }
}
