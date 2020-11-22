using Etimo.Id.Entities;
using System;
using System.Text.Json.Serialization;

namespace Etimo.Id.Api.Models
{
    public class ClientResponseDto
    {
        [JsonPropertyName("client_id")]
        public Guid ClientId { get; set; }

        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; set; }

        [JsonPropertyName("user_id")]
        public Guid UserId { get; set; }

        public static ClientResponseDto FromClient(Client client)
        {
            return new ClientResponseDto
            {
                ClientId = client.ClientId,
                RedirectUri = client.RedirectUri,
                UserId = client.UserId
            };
        }
    }
}
