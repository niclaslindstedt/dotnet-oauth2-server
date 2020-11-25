// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;
using System;
using System.Linq;

namespace Etimo.Id.Api.OAuth
{
    public class TokenRequestForm
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public Guid refresh_token { get; set; }
        public string scope { get; set; }

        public TokenRequest ToTokenRequest()
        {
            return new TokenRequest
            {
                GrantType = grant_type,
                ClientId = client_id,
                ClientSecret = client_secret,
                RefreshToken = refresh_token,
                Scope = scope?.Split(' ').ToList()
            };
        }
    }
}
