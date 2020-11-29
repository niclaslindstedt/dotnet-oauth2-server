// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Api.Users
{
    public class UserRequestDto
    {
        public string username { get; set; }

        // https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html#maximum-password-lengths
        [MaxLength(64)]
        public string password { get; set; }

        public User ToUser()
        {
            return new User
            {
                Username = username,
                Password = password
            };
        }
    }
}
