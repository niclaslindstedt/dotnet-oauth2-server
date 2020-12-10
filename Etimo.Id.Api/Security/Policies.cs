using Etimo.Id.Service.Constants;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Etimo.Id.Api.Security
{
    public class Policies
    {
        public static AuthorizationPolicy ScopePolicy(params string[] scopes)
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireAssertion(context =>
            {
                var hasAllClaims = true;
                foreach (var scope in scopes)
                {
                    var claim = context.User.FindFirst("scope");
                    var hasThisClaim = claim != null && claim.Value.Split(' ').Any(claimScope => claimScope == scope);
                    if (!hasThisClaim)
                    {
                        hasAllClaims = false;
                        break;
                    }
                }

                // The caller must either have the scope claims or be an admin.
                return hasAllClaims || context.User.IsInRole(RoleNames.Admin);
            }).Build();
        }
    }
}
