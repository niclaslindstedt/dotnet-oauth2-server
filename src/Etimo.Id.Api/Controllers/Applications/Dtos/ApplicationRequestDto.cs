// ReSharper disable InconsistentNaming

using Etimo.Id.Api.Attributes;
using Etimo.Id.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Etimo.Id.Api.Applications
{
    public class ApplicationRequestDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(32)]
        [NqsChar]
        public string name { get; set; }

        [MaxLength(128)]
        [NqsChar]
        public string description { get; set; }

        [Required]
        [ValidValues("public", "confidential")]
        public string type { get; set; }

        [Required]
        [ValidUri(allowFragment: true)]
        public string homepage_uri { get; set; }

        [Required]
        [ValidUri]
        public string redirect_uri { get; set; }

        public Guid? user_id { get; set; }

        [Base64]
        public string logo_base64 { get; set; }

        public Application ToApplication(int? applicationId = null)
            => new()
            {
                ApplicationId = applicationId ?? default,
                Name          = name,
                Description   = description,
                Type          = type,
                LogoBase64    = logo_base64,
                HomepageUri   = homepage_uri,
                RedirectUri   = redirect_uri,
                UserId        = user_id.GetValueOrDefault(),
            };
    }
}
