// ReSharper disable InconsistentNaming

using Etimo.Id.Exceptions;
using System;

namespace Etimo.Id.Dtos
{
    public class ErrorResponseDto
    {
        private readonly int _statusCode;

        public ErrorResponseDto(ErrorCodeException exception, bool addStackTrace = false)
        {
            error             = exception.ErrorCode;
            _statusCode       = exception.GetStatusCode();
            error_description = exception.Message;
            error_uri         = exception.GetStatusCodeUri();
            stack_trace       = addStackTrace ? exception.ToString() : null;
            state             = exception.State;
        }

        public string error             { get; set; }
        public string error_description { get; set; }
        public Uri    error_uri         { get; set; }
        public string stack_trace       { get; set; }
        public string state             { get; set; }

        public int GetStatusCode()
            => _statusCode;
    }
}
