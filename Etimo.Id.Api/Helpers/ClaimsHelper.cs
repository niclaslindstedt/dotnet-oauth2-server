using Etimo.Id.Service.Constants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Etimo.Id.Api.Helpers
{
    public static class ClaimsHelper
    {
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

        /// <summary>
        /// Gets the UserId from the list of claims that the requester has.
        /// </summary>
        public static bool UserHasRole(this Controller controller, string role)
        {
            var claimRoles = controller.Request.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role);

            return claimRoles.Any(r => r.Value == role);
        }

        /// <summary>
        /// Returns the highest privileged role the requester has.
        /// </summary>
        public static string UserRole(this Controller controller)
        {
            var claimRoles = controller.Request.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

            foreach (var role in Roles.InPrivilegeOrder())
            {
                if (claimRoles.Any(r => r.Value == Roles.Admin))
                {
                    return role;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the "aud" claim (ClientId).
        /// </summary>
        public static Guid GetClientId(this Controller controller)
        {
            var audClaim = controller.GetUserClaim(JwtRegisteredClaimNames.Aud);
            if (audClaim == null || !Guid.TryParse(audClaim, out var clientId))
            {
                throw new UnauthorizedAccessException("The access token lacks the 'aud' claim.");
            }

            return clientId;
        }

        /// <summary>
        /// Returns the "jti" claim (AccessTokenId).
        /// </summary>
        public static Guid GetAccessTokenId(this Controller controller)
        {
            var jitClaim = controller.GetUserClaim(JwtRegisteredClaimNames.Jti);
            if (jitClaim == null || !Guid.TryParse(jitClaim, out var accessTokenId))
            {
                throw new UnauthorizedAccessException("The access token lacks the 'jit' claim.");
            }

            return accessTokenId;
        }
    }
}
