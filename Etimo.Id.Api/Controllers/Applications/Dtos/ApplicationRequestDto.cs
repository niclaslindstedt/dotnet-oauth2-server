// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Attributes;
using Etimo.Id.Entities;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Api.Applications
{
    public class ApplicationRequestDto
    {
        [Required]
        [MinLength(2), MaxLength(32)]
        [NqsChar]
        public string name { get; set; }

        [MaxLength(128)]
        [NqsChar]
        public string description { get; set; }

        [Required]
        [ValidUri(allowFragment: true)]
        public string homepage_uri { get; set; }

        [Required]
        [ValidUri]
        public string redirect_uri { get; set; }

        public Application ToApplication(int? applicationId = null)
        {
            return new Application
            {
                ApplicationId = applicationId ?? default,
                Name = name,
                Description = description,
                HomepageUri = homepage_uri,
                RedirectUri = redirect_uri
            };
        }
    }
}
