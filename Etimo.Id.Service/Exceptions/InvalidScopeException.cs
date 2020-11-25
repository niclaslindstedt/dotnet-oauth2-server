namespace Etimo.Id.Service.Exceptions
{
    public class InvalidScopeException : BadRequestException
    {
        /// <summary>
        /// The requested scope is invalid, unknown, malformed, or
        /// exceeds the scope granted by the resource owner.
        /// </summary>
        public InvalidScopeException(string message) : base(message, "invalid_scope")
        {
        }
    }
}
