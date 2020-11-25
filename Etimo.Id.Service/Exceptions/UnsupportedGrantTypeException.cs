namespace Etimo.Id.Service.Exceptions
{
    public class UnsupportedGrantTypeException : BadRequestException
    {
        /// <summary>
        /// The authorization grant type is not supported by the
        /// authorization server.
        /// </summary>
        public UnsupportedGrantTypeException(string message) : base(message, "unsupported_grant_type")
        {
        }
    }
}
