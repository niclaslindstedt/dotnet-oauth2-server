using System;
using System.Collections.Generic;

namespace Etimo.Id.Exceptions
{
    public abstract class ErrorCodeException : Exception
    {
        protected ErrorCodeException(
            string message,
            string errorCode,
            Exception exception)
            : base(message, exception)
        {
            ErrorCode = errorCode;
        }

        protected ErrorCodeException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public string                     ErrorCode { get; set; }
        public Dictionary<string, string> Headers   { get; set; } = new();
        public string                     State     { get; set; }

        public int GetStatusCode()
            => this switch
            {
                BadRequestException      => 400,
                UnauthorizedException    => 401,
                ForbiddenException       => 403,
                NotFoundException        => 404,
                ConflictException        => 409,
                TooManyRequestsException => 429,
                _                        => 500,
            };

        public Uri GetStatusCodeUri()
            => GetStatusCode() switch
            {
                400 => new Uri("https://tools.ietf.org/html/rfc7231#section-6.5.1"),
                401 => new Uri("https://tools.ietf.org/html/rfc7235#section-3.1"),
                403 => new Uri("https://tools.ietf.org/html/rfc7231#section-6.5.3"),
                404 => new Uri("https://tools.ietf.org/html/rfc7231#section-6.5.4"),
                409 => new Uri("https://tools.ietf.org/html/rfc7231#section-6.5.8"),
                429 => new Uri("https://tools.ietf.org/html/rfc6585#section-4"),
                500 => new Uri("https://tools.ietf.org/html/rfc7231#section-6.6.1"),
                _   => null,
            };
    }
}
