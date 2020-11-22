using Etimo.Id.Entities;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Models
{
    public class NewUserRequestDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        public User ToUser()
        {
            return new User
            {
                Username = Username,
                Password = Password
            };
        }
    }
}
