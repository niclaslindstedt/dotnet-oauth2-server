// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;
using System;
using System.Linq;

namespace Etimo.Id.Api.OAuth
{
    public class AccessTokenRequestForm
    {
        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string redirect_uri { get; set; }
        public Guid? refresh_token { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string code { get; set; }
        public string scope { get; set; }

        public TokenRequest ToTokenRequest()
        {
            return new TokenRequest
            {
                GrantType = grant_type,
                ClientId = client_id,
                ClientSecret = client_secret,
                RedirectUri = redirect_uri,
                RefreshToken = refresh_token,
                Username = username,
                Password = password,
                Code = code,
                Scope = scope?.Split(' ').ToList()
            };
        }
    }
}
