namespace Etimo.Id.Service.Exceptions
{
    public class InvalidGrantException : ErrorCodeException
    {
        public InvalidGrantException() : base("invalid_grant", "invalid_grant")
        {
        }
    }
}
