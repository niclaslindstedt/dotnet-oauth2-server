using Etimo.Id.Entities.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etimo.Id.Entities
{
    public class TokenRequest
        : IClientCredentialsRequest,
            IUserCredentials,
            IAuthorizationCodeRequest,
            IResourceOwnerCredentialsRequest
    {
        /// <summary>
        /// Authorization Code Grant.
        /// https://tools.ietf.org/html/rfc6749#section-4.1.3
        /// </summary>
        public TokenRequest(string grantType, string code, string redirectUri, Guid clientId)
        {
            GrantType = grantType;
            Code = code;
            RedirectUri = redirectUri;
            ClientId = clientId;
        }

        /// <summary>
        /// Resource Owner Password Credentials Grant.
        /// https://tools.ietf.org/html/rfc6749#section-4.3.2
        /// </summary>
        public TokenRequest(string grantType, string username, string password, string scope)
        {
            GrantType = grantType;
            Username = username;
            Password = password;
            Scope = scope?.Split(" ").ToList();
        }

        /// <summary>
        /// Client Credentials Grant.
        /// https://tools.ietf.org/html/rfc6749#section-4.4.2
        /// </summary>
        public TokenRequest(string grantType, string scope)
        {
            GrantType = grantType;
            Scope = scope?.Split(" ").ToList();
        }

        /// <summary>
        /// Refresh Token Grant.
        /// https://tools.ietf.org/html/rfc6749#section-6
        /// </summary>
        public TokenRequest(string grantType, string refreshToken, string scope)
        {
            GrantType = grantType;
            RefreshToken = refreshToken;
            Scope = scope?.Split(" ").ToList();
        }

        public string GrantType { get; set; }
        public Guid ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string RefreshToken { get; set; }
        public List<string> Scope { get; set; }
        public string Code { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
