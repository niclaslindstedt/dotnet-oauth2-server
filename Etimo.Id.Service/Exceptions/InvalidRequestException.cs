namespace Etimo.Id.Service.Exceptions
{
    /// <summary>
    /// The request is missing a required parameter, includes an
    /// unsupported parameter value(other than grant type),
    /// repeats a parameter, includes multiple credentials,
    /// utilizes more than one mechanism for authenticating the
    /// client, or is otherwise malformed.
    /// </summary>
    public class InvalidRequestException : BadRequestException
    {
        public InvalidRequestException(string message) : base(message, "invalid_request")
        {
        }
    }
}
