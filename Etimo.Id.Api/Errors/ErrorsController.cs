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

        [Route("error")]
        public ErrorResponse Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            ErrorResponse response;
            if (exception is ErrorCodeException errorCodeException)
            {
                response = new ErrorResponse(errorCodeException);
            }
            else
            {
                response = new ErrorResponse(exception);
            }

            Response.StatusCode = response.GetStatusCode();

            if (!_environment.IsDevelopment())
            {
                response.StackTrace = null;
            }

            return response;
        }
    }
}
