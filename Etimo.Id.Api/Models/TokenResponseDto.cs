using Etimo.Id.Entities;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Models
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
        public string Scope { get; set; }

        public static TokenResponseDto FromJwtToken(JwtToken token)
        {
            return new TokenResponseDto
            {
                JwtToken = token.TokenBase64,
                TokenType = token.TokenType,
                ExpiresIn = token.ExpiresIn,
                RefreshToken = token.RefreshToken,
                Scope = token.Scopes != null ? string.Join(" ", token.Scopes) : null,
            };
        }
    }
}
