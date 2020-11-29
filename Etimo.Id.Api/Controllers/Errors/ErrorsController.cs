using Etimo.Id.Service.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Etimo.Id.Api.Errors
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ErrorsController(IWebHostEnvironment environment)
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
                response = new ErrorResponseDto(errorCodeException);
            }
            else
            {
                var addStackTrace = _environment.IsDevelopment();
                response = new ErrorResponseDto(exception, addStackTrace);
            }

            Response.StatusCode = response.GetStatusCode();

            return response;
        }
    }
}
