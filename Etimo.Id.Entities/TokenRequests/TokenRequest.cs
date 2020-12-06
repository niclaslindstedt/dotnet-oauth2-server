using Etimo.Id.Entities.Abstractions;
using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public abstract class TokenRequest : ITokenRequest
    {
        public string GrantType { get; set; }
        public Guid ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string RefreshToken { get; set; }
        public string Scope { get; set; }
        public string Code { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
