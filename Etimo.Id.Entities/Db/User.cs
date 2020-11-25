using Etimo.Id.Entities.Abstractions;
using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class User : IUser
    {
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Application> Applications { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
