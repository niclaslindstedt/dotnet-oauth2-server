using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class Scope
    {
        public Guid     ScopeId          { get; set; } = Guid.NewGuid();
        public string   Name             { get; set; }
        public string   Description      { get; set; }
        public int      ApplicationId    { get; set; }
        public DateTime CreatedDateTime  { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDateTime { get; set; } = DateTime.UtcNow;

        public virtual Application       Application { get; set; }
        public virtual ICollection<Role> Roles       { get; set; }

        public void Update(Scope scopeWithUpdates)
        {
            Name             = scopeWithUpdates.Name;
            Description      = scopeWithUpdates.Description;
            ModifiedDateTime = DateTime.UtcNow;
        }
    }
}
