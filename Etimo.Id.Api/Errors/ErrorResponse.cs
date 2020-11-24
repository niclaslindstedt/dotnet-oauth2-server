using Etimo.Id.Api.Helpers;
using Etimo.Id.Service.Exceptions;
using System;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Errors
{
    public class ErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }

        [JsonPropertyName("error_uri")]
        public Uri ErrorUri { get; set; }

        [JsonPropertyName("stack_trace")]
        public string StackTrace { get; set; }

        public ErrorResponse(Exception exception)
        {
            Error = exception.GetType().Name;
            Initialize(exception);
        }

        public ErrorResponse(ErrorCodeException exception)
        {
            Error = exception.ErrorCode;
            Initialize(exception);
        }

        private void Initialize(Exception exception)
        {
            _statusCode = exception.GetStatusCode();
            ErrorDescription = exception.Message;
            StackTrace = exception.ToString();
            ErrorUri = _statusCode.GetStatusCodeUri();
        }

        private int _statusCode;

        internal int GetStatusCode() => _statusCode;
    }
}
