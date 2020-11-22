using Etimo.Id.Entities;
using System;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Models
{
    public class NewClientResponseDto
    {
        [JsonPropertyName("client_id")]
        public Guid ClientId { get; set; }

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }

        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        public static NewClientResponseDto FromClient(Client client)
        {
            return new NewClientResponseDto
            {
                ClientId = client.ClientId,
                ClientSecret = client.ClientSecret,
                RedirectUri = client.RedirectUri,
                UserId = client.UserId
            };
        }
    }
}
