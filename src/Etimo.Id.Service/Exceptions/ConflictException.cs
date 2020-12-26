namespace Etimo.Id.Service.Exceptions
{
    public class ConflictException : ErrorCodeException
    {
        /// <summary>
        /// The user is trying to add something that already exists.
        /// </summary>
        public ConflictException(string message)
            : base(message, "conflict")
        {
        }
    }
}
