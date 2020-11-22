using Etimo.Id.Entities;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Models
{
    public class NewClientRequestDto
    {
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }

        public Client ToClient()
        {
            return new Client
            {
                RedirectUri = RedirectUri
            };
        }
    }
}
