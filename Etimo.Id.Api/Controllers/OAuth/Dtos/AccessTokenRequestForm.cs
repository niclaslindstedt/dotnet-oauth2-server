// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Constants;
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

        public Guid client_id { get; set; }

        [JsonIgnore]
        public string client_secret { get; set; }

        [Regex("^https://", "The redirect_uri must use TLS encryption (https). Read more: https://tools.ietf.org/html/rfc6749#section-3.1.2.1")]
        [Regex("^[^#]+$", "The redirect_uri cannot use fragments. Read more: https://tools.ietf.org/html/rfc6749#section-3.1.2")]
        public string redirect_uri { get; set; }

        public string refresh_token { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string code { get; set; }

        [Regex(CharacterSetPatterns.NQCHAR, "The scope field can only contain NQCHAR characters (%x21 / %x23-5B / %x5D-7E).")]
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
                    return new TokenRequest(grant_type, code, redirect_uri, client_id);

                case "refresh_token":
                    return new TokenRequest(grant_type, refresh_token, scope);

                default:
                    throw new UnsupportedGrantTypeException("Invalid grant type.");
            }
        }
    }
}
