namespace Etimo.Id.Service.Exceptions
{
    public class InvalidGrantException : BadRequestException
    {
        public InvalidGrantException(string message) : base(message, "invalid_grant")
        {
        }
    }
}
