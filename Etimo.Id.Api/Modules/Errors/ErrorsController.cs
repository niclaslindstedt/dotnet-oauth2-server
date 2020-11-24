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
            var addStackTrace = _environment.IsDevelopment();

            ErrorResponse response;
            if (exception is ErrorCodeException errorCodeException)
            {
                response = new ErrorResponse(errorCodeException, addStackTrace);
            }
            else
            {
                response = new ErrorResponse(exception, addStackTrace);
            }

            Response.StatusCode = response.GetStatusCode();

            return response;
        }
    }
}
