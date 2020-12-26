using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class Role
    {
        public Guid RoleId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public int ApplicationId { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDateTime { get; set; } = DateTime.UtcNow;

        public virtual Application Application { get; set; }
        public virtual ICollection<Scope> Scopes { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public void Update(Role roleWithUpdates)
        {
            Description = roleWithUpdates.Description;
            ModifiedDateTime = DateTime.UtcNow;
        }
    }
}
