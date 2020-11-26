using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class ClientCredentialsTokenGenerator : IClientCredentialsTokenGenerator
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public ClientCredentialsTokenGenerator(
            IApplicationsService applicationsService,
            IJwtTokenFactory jwtTokenFactory)
        {
            _applicationsService = applicationsService;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            var application = await _applicationsService.AuthenticateAsync(new Guid(request.ClientId), request.ClientSecret);

            request.ClientId = application.UserId.ToString();
            var jwtToken = _jwtTokenFactory.CreateJwtToken(request);

            return jwtToken;
        }
    }
}
