using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Settings;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service.Security
{
    internal class ClientCredentialsTokenGenerator : TokenGenerator
    {
        private readonly IClientsService _clientsService;

        public ClientCredentialsTokenGenerator(
            IClientsService clientsService,
            OAuthSettings settings) : base(settings)
        {
            _clientsService = clientsService;
        }

        public override async Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            var client = await _clientsService.AuthenticateAsync(new Guid(request.ClientId), request.ClientSecret);
            request.ClientId = client.UserId.ToString();

            return CreateJwtToken(request);
        }
    }
}
