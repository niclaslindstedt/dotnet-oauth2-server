// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Attributes;
using Etimo.Id.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Api.Users
{
    public class UserRequestDto
    {
        [Required]
        [NqChar]
        public string username { get; set; }

        // https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#maximum-password-lengths
        [MaxLength(64)]
        public string password { get; set; }

        public User ToUser(Guid? userId = null)
        {
            var user = new User
            {
                Username = username,
                Password = password
            };

            if (userId != null)
            {
                user.UserId = userId.Value;
            }

            return user;
        }
    }
}
