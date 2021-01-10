using Etimo.Id.Api.Bootstrapping;
using Etimo.Id.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Errors
{
    public class ErrorMiddleware
    {
        private readonly IWebHostEnvironment _environment;
        private readonly RequestDelegate     _next;

        public ErrorMiddleware(RequestDelegate next, IWebHostEnvironment environment)
        {
            _next        = next;
            _environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try { await _next.Invoke(context); }
            catch (Exception exception)
            {
                ErrorResponseDto response;
                if (exception is ErrorCodeException errorCodeException)
                {
                    foreach ((string key, string value) in errorCodeException.Headers) { context.Response.Headers.Add(key, value); }

                    response = new ErrorResponseDto(errorCodeException);
                }
                else
                {
                    bool addStackTrace        = _environment.IsDevelopment();
                    var  serverErrorException = new ServerErrorException(exception?.Message, exception);
                    response = new ErrorResponseDto(serverErrorException, addStackTrace);
                }

                context.Response.StatusCode = response.GetStatusCode();
                context.Response.Headers.Add("Content-Type", "application/json");

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(
                        response,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
                            IgnoreNullValues     = true,
                        }));
            }
        }
    }


    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorMiddleware(this IApplicationBuilder builder)
        {
            var env = builder.ApplicationServices.GetService(typeof(IWebHostEnvironment)) as IWebHostEnvironment;

            return builder.UseMiddleware<ErrorMiddleware>(env);
        }
    }
}
