// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Applications;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Etimo.Id.Api.Users
{
    public class UserResponseDto
    {
        public Guid user_id { get; set; }
        public string username { get; set; }
        public DateTime created_date { get; set; }
        public DateTime modified_date { get; set; }
        public List<ApplicationResponseDto> applications { get; set; }

        public static UserResponseDto FromUser(User user) => FromUser(user, true);
        public static UserResponseDto FromUser(User user, bool includeChildren)
        {
            var dto = new UserResponseDto
            {
                user_id = user.UserId,
                username = user.Username,
                created_date = user.CreatedDateTime,
                modified_date = user.ModifiedDateTime
            };

            if (includeChildren)
            {
                dto.applications = user.Applications?.Select(a =>
                    ApplicationResponseDto.FromApplication(a, false)).ToList();
            }

            return dto;
        }
    }
}
