using System.Text.Json.Serialization;

namespace Etimo.Id.Models
{
    public class TokenRequestDto
    {
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
