using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Etimo.Id.Api.Helpers
{
    public static class AuthenticationHelper
    {
        public static bool UserIsAuthenticated(this Controller controller)
        {
            IIdentity? identity = controller.User?.Identity;

            return identity != null && identity.IsAuthenticated;
        }
    }
}
