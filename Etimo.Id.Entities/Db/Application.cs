using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public Guid ClientId { get; set; } = Guid.NewGuid();
        public string ClientSecret { get; set; }
        public string HomepageUri { get; set; }
        //TODO: Add support for multiple redirect uris
        public string RedirectUri { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDateTime { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }

        /// <summary>
        /// Updates the current application with values from otherApplication.
        /// </summary>
        /// <param name="otherApplication">The application to fetch values from.</param>
        public void MergeWith(Application otherApplication)
        {
            Name = otherApplication.Name;
            Description = otherApplication.Description;
            Type = otherApplication.Type;
            HomepageUri = otherApplication.HomepageUri;
            RedirectUri = otherApplication.RedirectUri;
        }
    }
}
