using Etimo.Id.Api.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Etimo.Id.Api.Helpers
{
    public static class HeaderHelper
    {
        /// <summary>
        /// Gets the SuperAdminKey value from the headers.
        /// </summary>
        public static string SuperAdminKeyHeader(this Controller controller)
        {
            if (controller.Request.Headers.ContainsKey(HeaderKeys.SuperAdminKey))
            {
                return controller.Request.Headers[HeaderKeys.SuperAdminKey].FirstOrDefault();
            }

            return null;
        }
    }
}
