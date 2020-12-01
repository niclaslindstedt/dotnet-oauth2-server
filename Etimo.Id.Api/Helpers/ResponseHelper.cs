using Microsoft.AspNetCore.Http;

namespace Etimo.Id.Api.Helpers
{
    public static class ResponseHelper
    {
        /// <summary>
        /// Prevent clickjacking. Read more: https://tools.ietf.org/html/rfc6749#section-10.13
        /// </summary>
        /// <param name="response"></param>
        public static void PreventClickjacking(this HttpResponse response)
        {
            response.Headers.Add("X-Frame-Options", "deny");
        }
    }
}
