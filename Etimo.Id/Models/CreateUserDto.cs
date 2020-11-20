using System.Text.Json.Serialization;

namespace Etimo.Id.Models
{
    public class CreateUserDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
