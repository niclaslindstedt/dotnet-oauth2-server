// ReSharper disable InconsistentNaming

using Etimo.Id.Service.Exceptions;
using System;

namespace Etimo.Id.Api.Errors
{
    public class ErrorResponseDto
    {
        public string error { get; set; }
        public string error_description { get; set; }
        public Uri error_uri { get; set; }
        public string stack_trace { get; set; }

        public ErrorResponseDto(Exception exception, bool addStackTrace = false)
        {
            error = exception.GetType().Name;
            Initialize(exception, addStackTrace);
        }

        public ErrorResponseDto(ErrorCodeException exception, bool addStackTrace = false)
        {
            error = exception.ErrorCode;
            Initialize(exception, addStackTrace);
        }

        private void Initialize(Exception exception, bool addStackTrace)
        {
            _statusCode = exception.GetStatusCode();
            error_description = exception.Message;
            error_uri = _statusCode.GetStatusCodeUri();
            stack_trace = addStackTrace ? exception.ToString() : null;
        }

        private int _statusCode;

        internal int GetStatusCode() => _statusCode;
    }
}
