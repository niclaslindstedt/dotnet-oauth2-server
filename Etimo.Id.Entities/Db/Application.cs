using System;
using System.Collections.Generic;

namespace Etimo.Id.Entities
{
    public class Application
    {
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ClientId { get; set; } = Guid.NewGuid();
        public string ClientSecret { get; set; }
        public string HomepageUri { get; set; }
        //TODO: Add support for multiple redirect uris
        public string RedirectUri { get; set; }
        public Guid UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

        /// <summary>
        /// Updates the current application with values from otherApplication.
        /// </summary>
        /// <param name="otherApplication">The application to fetch values from.</param>
        public void MergeWith(Application otherApplication)
        {
            Name = otherApplication.Name;
            Description = otherApplication.Description;
            HomepageUri = otherApplication.HomepageUri;
            RedirectUri = otherApplication.RedirectUri;
        }
    }
}
