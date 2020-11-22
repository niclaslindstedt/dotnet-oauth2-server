using Etimo.Id.Entities;
using System;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Models
{
    public class UserResponseDto
    {
        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        public static UserResponseDto FromUser(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username
            };
        }
    }
}
