using Microsoft.AspNetCore.Mvc;

namespace Etimo.Id.Api.Helpers
{
    public static class AuthenticationHelper
    {
        public static bool UserIsAuthenticated(this Controller controller)
        {
            var identity = controller.User?.Identity;

            return identity != null && identity.IsAuthenticated;
        }
    }
}
