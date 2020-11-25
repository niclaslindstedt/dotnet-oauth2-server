using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class TokenRequest
    {
        public string GrantType { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Guid? RefreshToken { get; set; }
        public List<string> Scope { get; set; }
        public Guid? UserId { get; set; }
    }
}
