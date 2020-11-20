using System;
using System.Text.Json.Serialization;

namespace Etimo.Id.Models
{
    public class UserDto
    {
        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
