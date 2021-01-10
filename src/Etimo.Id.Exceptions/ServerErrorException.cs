using System;

namespace Etimo.Id.Exceptions
{
    public class ServerErrorException : BadRequestException
    {
        /// <summary>
        ///     The authorization server encountered an unexpected condition that prevented it from fulfilling the request.
        ///     Read more: https://tools.ietf.org/html/rfc6749#section-4.1.2.1
        /// </summary>
        public ServerErrorException(
            string message,
            Exception exception,
            string state = null)
            : base(message, "server_error", exception)
        {
            State = state;
        }
    }
}
