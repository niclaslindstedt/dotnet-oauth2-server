using Etimo.Id.Service.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text;

namespace Etimo.Id.Api.Helpers
{
    public static class BasicAuthenticationHelper
    {
        /// <summary>
        /// Checks the request headers for basic authentication.
        /// </summary>
        /// <param name="request">The HTTP request in question.</param>
        /// <returns>True if the request is using basic authentication, false if not.</returns>
        public static bool IsBasicAuthentication(this HttpRequest request)
        {
            if (!request.Headers.ContainsKey("Authorization"))
            {
                return false;
            }

            var header = request.Headers["Authorization"];
            var type = header.First().Split(' ')[0].ToLowerInvariant();
            return type == "basic";

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetBasicAuthenticationCredentials(this HttpRequest request)
        {
            try
            {
                var header = request.Headers["Authorization"];
                var parts = header.First().Split(' ');
                var authBytes = Convert.FromBase64String(parts[1]);
                var authString = Encoding.UTF8.GetString(authBytes);
                var authParts = authString.Split(':');
                return new Tuple<string, string>(authParts[0], authParts[1]);
            }
            catch
            {
                throw new InvalidRequestException();
            }
        }
    }
}
