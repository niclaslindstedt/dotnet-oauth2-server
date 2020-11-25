// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;
using System;

namespace Etimo.Id.Api.OAuth
{
    public class AuthorizationRequestForm
    {
        public string response_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string redirect_uri { get; set; }
        public string scope { get; set; }
        public string state { get; set; }

        public AuthorizationRequest ToAuthorizeRequest()
        {
            return new AuthorizationRequest
            {
                ResponseType = response_type,
                ClientId = client_id != null ? (Guid?)new Guid(client_id) : null,
                ClientSecret = client_secret,
                RedirectUri = redirect_uri,
                Scope = scope,
                State = state
            };
        }
    }
}
