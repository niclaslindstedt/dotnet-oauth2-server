using Etimo.Id.Service.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Middleware
{
    public class BruteForceProtectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;
        private readonly int _banTimeInMinutes = 30;

        private static readonly IDictionary<string, DateTime> Banned = new ConcurrentDictionary<string, DateTime>();

        public BruteForceProtectionMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            if (ip == null)
            {
                await _next(context);
                return;
            }

            try
            {
                if (Banned.ContainsKey(ip) && Banned[ip] > DateTime.UtcNow)
                {
                    throw new TooManyRequestsException("Too many requests", _banTimeInMinutes * 60);
                }

                await _next(context);
            }
            catch (InvalidGrantException)
            {
                await HandleAsync(ip, context);
                throw;
            }
            catch (UnauthorizedException)
            {
                await HandleAsync(ip, context);
                throw;
            }
            catch (ForbiddenException)
            {
                await HandleAsync(ip, context);
                throw;
            }
        }

        private async Task HandleAsync(string ip, HttpContext context)
        {
            var requestsString = await _cache.GetStringAsync(ip);
            if (!string.IsNullOrEmpty(requestsString))
            {
                var requests = int.Parse(requestsString);
                if (requests > 2)
                {
                    Banned[ip] = DateTime.UtcNow.AddMinutes(_banTimeInMinutes);
                }

                requests++;

                await _cache.SetStringAsync(ip, requests.ToString());
            }
            else
            {
                await _cache.SetStringAsync(ip, "1", new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(30)
                });
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class BruteForceProtectionMiddlewareExtensions
    {
        public static IApplicationBuilder UseBruteForceProtection(this IApplicationBuilder builder)
        {
            var cache = builder.ApplicationServices.GetService(typeof(IDistributedCache));

            return builder.UseMiddleware<BruteForceProtectionMiddleware>(cache);
        }
    }
}
