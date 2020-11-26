using Etimo.Id.Service.Exceptions;
using System;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Errors
{
    public class ErrorResponseDto
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }

        [JsonPropertyName("error_uri")]
        public Uri ErrorUri { get; set; }

        [JsonPropertyName("stack_trace")]
        public string StackTrace { get; set; }

        public ErrorResponseDto(Exception exception, bool addStackTrace = false)
        {
            Error = exception.GetType().Name;
            Initialize(exception, addStackTrace);
        }

        public ErrorResponseDto(ErrorCodeException exception, bool addStackTrace = false)
        {
            Error = exception.ErrorCode;
            Initialize(exception, addStackTrace);
        }

        private void Initialize(Exception exception, bool addStackTrace)
        {
            _statusCode = exception.GetStatusCode();
            ErrorDescription = exception.Message;
            ErrorUri = _statusCode.GetStatusCodeUri();
            StackTrace = addStackTrace ? exception.ToString() : null;
        }

        private int _statusCode;

        internal int GetStatusCode() => _statusCode;
    }
}
