using Etimo.Id.Service.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Etimo.Id.Api.Errors
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ErrorController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [Route("/error")]
        public ErrorResponseDto Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            ErrorResponseDto response;
            if (exception is ErrorCodeException errorCodeException)
            {
                foreach (var (key, value) in errorCodeException.Headers)
                {
                    Response.Headers.Add(key, value);
                }

                response = new ErrorResponseDto(errorCodeException);
            }
            else
            {
                var addStackTrace = _environment.IsDevelopment();
                var serverErrorException = new ServerErrorException(exception?.Message, exception);
                response = new ErrorResponseDto(serverErrorException, addStackTrace);
            }

            Response.StatusCode = response.GetStatusCode();

            return response;
        }
    }
}
