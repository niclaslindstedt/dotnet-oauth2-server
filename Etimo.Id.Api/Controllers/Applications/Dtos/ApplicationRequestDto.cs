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
        public string name { get; set; }

        [MaxLength(128)]
        public string description { get; set; }

        [Required]
        [Regex("^https?://", "The homepage_uri must have a specified protocol (http or https).")]
        public string homepage_uri { get; set; }

        [Required]
        [Regex("^https://", "The redirect_uri must use TLS encryption (https). Read more: https://tools.ietf.org/html/rfc6749#section-3.1.2.1")]
        [Regex("^[^#]+$", "The redirect_uri cannot use fragments. Read more: https://tools.ietf.org/html/rfc6749#section-3.1.2")]
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
