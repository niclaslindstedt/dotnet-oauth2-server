using System;

namespace Etimo.Id.Entities
{
    /// <summary>
    /// This data object is used by the authorization code grant flow to populate the
    /// login screen and redirect the user back to the client's callback url.
    /// </summary>
    public class AuthorizationResponse
    {
        public string ResponseType { get; set; }
        public Guid ClientId { get; set; }
        public string State { get; set; }
        public string RedirectUri { get; set; }
        public Guid AuthorizationCodeId { get; set; }

        public string ToQueryParameters()
        {
            return $"response_type={ResponseType}" +
                   $"&client_id={ClientId}" +
                   $"&redirect_uri={RedirectUri}" +
                   $"&state={State}" +
                   $"&code_id={AuthorizationCodeId}";
        }
    }
}
