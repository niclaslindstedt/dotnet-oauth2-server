using Etimo.Id.Entities;
using System;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Models
{
    public class UserDto
    {
        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        public static UserDto FromUser(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username
            };
        }
    }
}
