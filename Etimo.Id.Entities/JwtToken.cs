using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class JwtToken
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public List<string> Scopes { get; set; }
    }
}
