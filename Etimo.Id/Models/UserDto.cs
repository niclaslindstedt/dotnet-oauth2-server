using System.Text.Json.Serialization;

namespace Etimo.Id.Models
{
    public class UserDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        [JsonIgnore]
        public string Password { get; set; }
    }
}
