using Etimo.Id.Api.Helpers;
using Etimo.Id.Constants;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Middleware
{
    public class RequestContextMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRequestContext requestContext)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                string userId = context.Request.GetUserClaimValue(ClaimTypes.NameIdentifier);
                if (userId != null) { requestContext.UserId = new Guid(userId); }

                string scope = context.Request.GetUserClaimValue(CustomClaimTypes.Scope);
                if (scope != null) { requestContext.Scopes = scope.Split(" ").ToList(); }

                string clientId = context.Request.GetUserClaimValue(JwtRegisteredClaimNames.Azp);
                if (clientId != null) { requestContext.ClientId = new Guid(clientId); }

                string username = context.Request.GetUserClaimValue(OpenIdConnectClaimTypes.PreferredUsername);
                if (username != null) { requestContext.Username = username; }
            }

            requestContext.IpAddress = context.Connection.RemoteIpAddress.ToString();

            await _next(context);
        }
    }


    public static class RequestContextMiddlewareExtensions
    {
        public static IServiceCollection AddRequestContext(this IServiceCollection services)
        {
            services.AddScoped<IRequestContext, RequestContext>();

            return services;
        }

        public static IApplicationBuilder UseRequestContext(this IApplicationBuilder builder)
            => builder.UseMiddleware<RequestContextMiddleware>();
    }
}
