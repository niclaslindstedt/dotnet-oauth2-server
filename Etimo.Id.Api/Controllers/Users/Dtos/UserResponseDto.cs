using Etimo.Id.Entities;
using System;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Users
{
    public class UserResponseDto
    {
        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("created_date")]
        public DateTime CreatedDateTime { get; set; }

        [JsonPropertyName("modified_date")]
        public DateTime ModifiedDateTime { get; set; }

        public static UserResponseDto FromUser(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                CreatedDateTime = user.CreatedDateTime,
                ModifiedDateTime = user.ModifiedDateTime
            };
        }
    }
}
