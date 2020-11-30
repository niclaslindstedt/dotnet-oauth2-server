// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Attributes;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.OAuth
{
    public class AccessTokenRequestForm
    {
        [Required]
        [ValidValues("password", "client_credentials", "authorization_code", "refresh_token")]
        public string grant_type { get; set; }

        public Guid? client_id { get; set; }

        [JsonIgnore]
        public string client_secret { get; set; }

        [ValidUri]
        public string redirect_uri { get; set; }

        [VsChar]
        public string refresh_token { get; set; }

        [Unicode]
        public string username { get; set; }

        [Unicode]
        public string password { get; set; }

        [VsChar]
        public string code { get; set; }

        [NqChar]
        public string scope { get; set; }

        public TokenRequest ToTokenRequest()
        {
            switch (grant_type)
            {
                case "password":
                    return new TokenRequest(grant_type, username, password, scope);

                case "client_credentials":
                    return new TokenRequest(grant_type, scope);

                case "authorization_code":
                    return new TokenRequest(grant_type, code, redirect_uri, client_id ?? Guid.Empty);

                case "refresh_token":
                    return new TokenRequest(grant_type, refresh_token, scope);

                default:
                    throw new UnsupportedGrantTypeException("Invalid grant type.");
            }
        }
    }
}
