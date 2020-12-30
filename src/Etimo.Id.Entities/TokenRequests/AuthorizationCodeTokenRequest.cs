using System;

namespace Etimo.Id.Entities
{
    public class AuthorizationCodeTokenRequest : TokenRequest
    {
        /// <summary>
        ///     Authorization Code Grant.
        ///     https://tools.ietf.org/html/rfc6749#section-4.1.3
        /// </summary>
        public AuthorizationCodeTokenRequest(
            string code,
            string redirectUri,
            Guid clientId)
        {
            GrantType   = "authorization_code";
            Code        = code;
            RedirectUri = redirectUri;
            ClientId    = clientId;
        }
    }
}
