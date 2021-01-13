using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;

namespace Etimo.Id.Client
{
    public class Policies
    {
        public static AuthorizationPolicy ScopePolicy(params string[] scopes)
            => new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .RequireAssertion(
                    context =>
                    {
                        var hasAllClaims = true;
                        foreach (string scope in scopes)
                        {
                            Claim claim        = context.User.FindFirst("scope");
                            bool  hasThisClaim = claim != null && claim.Value.Split(' ').Any(claimScope => claimScope == scope);
                            if (!hasThisClaim)
                            {
                                hasAllClaims = false;
                                break;
                            }
                        }

                        return hasAllClaims;
                    })
                .Build();
    }
}
