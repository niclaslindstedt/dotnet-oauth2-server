using System;

namespace Etimo.Id.Entities
{
    /// <summary>
    /// This data object is used by the authorization code grant flow to populate the
    /// login screen and redirect the user back to the client's callback url.
    /// </summary>
    public class AuthorizationResponse
    {
        public string Code { get; set; }
        public string State { get; set; }
        public string RedirectUri { get; set; }

        public Uri ToUri()
        {
            return new Uri($"{RedirectUri}?code={Code}&state={State}");
        }
    }
}
