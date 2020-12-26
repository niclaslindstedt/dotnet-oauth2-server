namespace Etimo.Id.Service.Exceptions
{
    public class AccessDeniedException : BadRequestException
    {
        /// <summary>
        /// The resource owner or authorization server denied the request.
        /// Read more: https://tools.ietf.org/html/rfc6749#section-4.1.2.1
        /// </summary>
        public AccessDeniedException(string message, string state = null)
            : base(message, "access_denied")
        {
            State = state;
        }
    }
}
