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
            switch (exception)
            {
                case InvalidGrantException ex:
                    response = new ErrorResponse(ex);
                    Response.StatusCode = 400;
                    break;

                case BadRequestException ex:
                    response = new ErrorResponse(ex);
                    Response.StatusCode = 400;
                    break;

                case UnauthorizedException ex:
                    response = new ErrorResponse(ex);
                    Response.StatusCode = 401;
                    break;


                case ForbiddenException ex:
                    response = new ErrorResponse(ex);
                    Response.StatusCode = 403;
                    break;


                case NotFoundException ex:
                    response = new ErrorResponse(ex);
                    Response.StatusCode = 404;
                    break;


                case ConflictException ex:
                    response = new ErrorResponse(ex);
                    Response.StatusCode = 409;
                    break;

                default:
                    response = new ErrorResponse(exception);
                    Response.StatusCode = 500;
                    break;
            }

            if (!_environment.IsDevelopment())
            {
                response.StackTrace = null;
            }

            return response;
        }
    }
}
