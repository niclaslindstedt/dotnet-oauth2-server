using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service.OAuth
{
    internal class ClientCredentialsTokenGenerator : TokenGenerator
    {
        private readonly IApplicationsService _applicationsService;

        public ClientCredentialsTokenGenerator(
            IApplicationsService applicationsService,
            OAuthSettings settings) : base(settings)
        {
            _applicationsService = applicationsService;
        }

        public override async Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            var client = await _applicationsService.AuthenticateAsync(new Guid(request.ClientId), request.ClientSecret);
            request.ClientId = client.UserId.ToString();

            return CreateJwtToken(request);
        }
    }
}
