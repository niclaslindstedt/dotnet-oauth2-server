using Etimo.Id.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Users
{
    public class UserRequestDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        // https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#maximum-password-lengths
        [MaxLength(64)]
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
