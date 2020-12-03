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
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDateTime { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Application> Applications { get; set; }
    }
}
