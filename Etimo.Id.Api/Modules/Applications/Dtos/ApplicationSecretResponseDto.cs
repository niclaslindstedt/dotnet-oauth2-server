using Etimo.Id.Entities;
using System;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Applications
{
    public class ApplicationSecretResponseDto
    {
        [JsonPropertyName("application_id")]
        public int ApplicationId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("homepage_uri")]
        public string HomepageUri { get; set; }

        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }

        [JsonPropertyName("client_id")]
        public Guid ClientId { get; set; }

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        public static ApplicationSecretResponseDto FromApplication(Application application)
        {
            return new ApplicationSecretResponseDto
            {
                ApplicationId = application.ApplicationId,
                Name = application.Name,
                Description = application.Description,
                HomepageUri = application.HomepageUri,
                RedirectUri = application.RedirectUri,
                ClientId = application.ClientId,
                ClientSecret = application.ClientSecret,
                UserId = application.UserId
            };
        }
    }
}
