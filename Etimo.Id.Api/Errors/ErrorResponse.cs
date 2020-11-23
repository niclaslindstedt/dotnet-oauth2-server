using Etimo.Id.Service.Exceptions;
using System;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Errors
{
    public class ErrorResponse
    {
        [JsonPropertyName("error_code")]
        public string ErrorCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("stack_trace")]
        public string StackTrace { get; set; }

        public ErrorResponse(Exception ex)
        {
            Message = ex.Message;
            StackTrace = ex.ToString();
        }

        public ErrorResponse(ErrorCodeException ex)
        {
            ErrorCode = ex.ErrorCode;
            Message = ex.Message;
            StackTrace = ex.ToString();
        }
    }
}
