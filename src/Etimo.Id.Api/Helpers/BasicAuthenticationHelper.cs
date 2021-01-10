using Etimo.Id.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Text;

namespace Etimo.Id.Api.Helpers
{
    public static class BasicAuthenticationHelper
    {
        /// <summary>
        ///     Checks the request headers for basic authentication.
        /// </summary>
        /// <param name="request">The HTTP request in question.</param>
        /// <returns>True if the request is using basic authentication, false if not.</returns>
        public static bool IsBasicAuthentication(this HttpRequest request)
        {
            if (!request.Headers.ContainsKey("Authorization")) { return false; }

            StringValues header = request.Headers["Authorization"];
            string       type   = header.First().Split(' ')[0].ToLowerInvariant();
            return type == "basic";
        }

        /// <summary>
        ///     Returns basic authentication credentials from Authorization header.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetCredentialsFromAuthorizationHeader(this HttpRequest request)
        {
            try
            {
                StringValues header     = request.Headers["Authorization"];
                string[]     parts      = header.First().Split(' ');
                byte[]       authBytes  = Convert.FromBase64String(parts[1]);
                string       authString = Encoding.UTF8.GetString(authBytes);
                string[]     authParts  = authString.Split(':');
                return new Tuple<string, string>(authParts[0], authParts[1]);
            }
            catch { throw new InvalidRequestException("Invalid basic authentication syntax."); }
        }
    }
}
