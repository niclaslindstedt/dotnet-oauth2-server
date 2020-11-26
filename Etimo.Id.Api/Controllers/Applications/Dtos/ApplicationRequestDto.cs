using Etimo.Id.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Applications
{
    public class ApplicationRequestDto
    {
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("homepage_uri")]
        public string HomepageUri { get; set; }

        [Required]
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }

        public Application ToApplication(int? applicationId = null)
        {
            return new Application
            {
                ApplicationId = applicationId ?? default,
                Name = Name,
                Description = Description,
                HomepageUri = HomepageUri,
                RedirectUri = RedirectUri
            };
        }
    }
}
