using Etimo.Id.Service.Constants;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace Etimo.Id.Api.Security
{
    public class Policies
    {
        public const string Admin = Roles.Admin;
        public const string User = Roles.User;

        public static AuthorizationPolicy AdminPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build();
        }

        public static AuthorizationPolicy UserPolicy()
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(User).Build();
        }

        public static AuthorizationPolicy ScopePolicy(string scope)
        {
            return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireAssertion(context =>
            {
                var claim = context.User.FindFirst("scope");
                var hasClaim = claim != null && claim.Value.Split(' ').Any(claimScope => claimScope == scope);

                // The caller must either have the scope claim or be an admin.
                return hasClaim || context.User.IsInRole(Roles.Admin);
            }).Build();
        }
    }
}
