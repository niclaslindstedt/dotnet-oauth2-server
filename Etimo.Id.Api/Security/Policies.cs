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

                return hasAllClaims;
            }).Build();
        }
    }
}
