using Etimo.Id.Entities.Abstractions;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class JwtTokenRequest : IJwtTokenRequest
    {
        public string Subject { get; set; }
        public List<string> Audience { get; set; }
    }
}
