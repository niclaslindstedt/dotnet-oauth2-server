using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Etimo.Id.Models
{
    public class TokenResponseDto
    {
        [JsonPropertyName("access_token")]
        public string JwtToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("scope")]
        public string Scope {
            get => _scope != null ? string.Join(" ", _scope) : null;
            set => _scope = value.Split(" ").ToList();
        }
        private List<string> _scope;
    }
}
