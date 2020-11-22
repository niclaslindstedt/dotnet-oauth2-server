using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Etimo.Id.Api.Helpers
{
    public static class ClaimsHelper
    {
        public static bool UserHasClaim(this Controller controller, string claim)
        {
            return controller.Request.HttpContext.User.Claims.Any(c => c.Value == claim);
        }

        public static string GetUserClaim(this Controller controller, string claim)
        {
            return controller.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == claim)?.Value;
        }

        /// <summary>
        /// Gets the UserId from the list of claims that the requester has.
        /// </summary>
        public static Guid GetUserId(this Controller controller)
        {
            var userId = controller.GetUserClaim(ClaimTypes.NameIdentifier);

            return userId != null ? new Guid(userId) : Guid.Empty;
        }
    }
}
