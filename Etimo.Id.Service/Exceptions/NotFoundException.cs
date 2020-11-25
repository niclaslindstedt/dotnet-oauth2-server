namespace Etimo.Id.Service.Exceptions
{
    public class NotFoundException : ErrorCodeException
    {
        /// <summary>
        /// The requested resource could not be found.
        /// </summary>
        public NotFoundException(string message = null) : base(message, "not_found")
        {
        }
    }
}