using Etimo.Id.Exceptions;
using System;

namespace Etimo.Id.Api.Errors
{
    public static class StatusCodeHelper
    {
        public static int GetStatusCode(this Exception ex)
            => ex switch
            {
                BadRequestException      => 400,
                UnauthorizedException    => 401,
                ForbiddenException       => 403,
                NotFoundException        => 404,
                ConflictException        => 409,
                TooManyRequestsException => 429,
                _                        => 500,
            };

        public static Uri GetStatusCodeUri(this int statusCode)
            => statusCode switch
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
