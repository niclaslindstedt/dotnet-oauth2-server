// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;
using System;

namespace Etimo.Id.Api.Users
{
    public class UserResponseDto
    {
        public Guid user_id { get; set; }
        public string username { get; set; }
        public DateTime created_date { get; set; }
        public DateTime modified_date { get; set; }

        public static UserResponseDto FromUser(User user)
        {
            return new UserResponseDto
            {
                user_id = user.UserId,
                username = user.Username,
                created_date = user.CreatedDateTime,
                modified_date = user.ModifiedDateTime
            };
        }
    }
}
