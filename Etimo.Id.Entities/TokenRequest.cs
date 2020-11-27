using Etimo.Id.Entities.Abstractions;
using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class TokenRequest
        : IClientCredentialsRequest,
            IUserCredentials,
            IAuthorizationCodeRequest,
            IResourceOwnerCredentialsRequest
    {
        public string GrantType { get; set; }
        public string ClientId { get; set; }
        public string Subject { get; set; }
        public string Audience { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public Guid? RefreshToken { get; set; }
        public List<string> Scope { get; set; }
        public string Code { get; set; }
        public Guid? UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
