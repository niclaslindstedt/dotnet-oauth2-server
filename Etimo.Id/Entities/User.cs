using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
