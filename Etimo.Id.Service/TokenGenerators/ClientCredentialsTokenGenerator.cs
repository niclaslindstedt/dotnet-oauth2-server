using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
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

        public async Task<JwtToken> GenerateTokenAsync(IClientCredentialsRequest request)
        {
            ValidateRequest(request);

            var application = await _applicationsService.AuthenticateAsync(request.ClientId, request.ClientSecret);

            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { application.ClientId.ToString() },
                Subject = application.UserId.ToString()
            };

            var jwtToken = _jwtTokenFactory.CreateJwtToken(jwtRequest);

            return jwtToken;
        }

        private static void ValidateRequest(IClientCredentialsRequest request)
        {
            if (request.ClientId == Guid.Empty || request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }
        }
    }
}
