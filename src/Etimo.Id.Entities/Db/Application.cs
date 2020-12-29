using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class Application
    {
        public int      ApplicationId    { get; set; }
        public string   Name             { get; set; }
        public string   Description      { get; set; }
        public string   Type             { get; set; }
        public Guid     ClientId         { get; set; } = Guid.NewGuid();
        public string   ClientSecret     { get; set; }
        public string   LogoBase64       { get; set; }
        public string   HomepageUri      { get; set; }
        public string   RedirectUri      { get; set; }
        public Guid     UserId           { get; set; }
        public DateTime CreatedDateTime  { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDateTime { get; set; } = DateTime.UtcNow;

        public virtual User               User   { get; set; }
        public virtual ICollection<Scope> Scopes { get; set; }
        public virtual ICollection<Role>  Roles  { get; set; }

        public void Update(Application applicationWithUpdates)
        {
            Name             = applicationWithUpdates.Name;
            Description      = applicationWithUpdates.Description;
            Type             = applicationWithUpdates.Type;
            LogoBase64       = applicationWithUpdates.LogoBase64;
            HomepageUri      = applicationWithUpdates.HomepageUri;
            RedirectUri      = applicationWithUpdates.RedirectUri;
            ModifiedDateTime = DateTime.UtcNow;
        }
    }
}
