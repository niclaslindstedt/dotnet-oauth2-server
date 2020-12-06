using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class ClientCredentialsTokenGenerator : IClientCredentialsTokenGenerator
    {
        private readonly IApplicationService _applicationService;
        private readonly IAccessTokensRepository _accessTokensRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public ClientCredentialsTokenGenerator(
            IApplicationService applicationService,
            IAccessTokensRepository accessTokensRepository,
            IJwtTokenFactory jwtTokenFactory)
        {
            _applicationService = applicationService;
            _accessTokensRepository = accessTokensRepository;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(IClientCredentialsTokenRequest request)
        {
            ValidateRequest(request);

            var application = await _applicationService.AuthenticateAsync(request.ClientId, request.ClientSecret);
            if (application.Type == ClientTypes.Public)
            {
                throw new UnauthorizedClientException("Public clients cannot use the client credentials grant.");
            }

            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { application.ClientId.ToString() },
                Subject = application.UserId.ToString()
            };

            var jwtToken = _jwtTokenFactory.CreateJwtToken(jwtRequest);

            _accessTokensRepository.Add(jwtToken.ToAccessToken());
            await _accessTokensRepository.SaveAsync();

            return jwtToken;
        }

        private static void ValidateRequest(IClientCredentialsTokenRequest request)
        {
            if (request.ClientId == Guid.Empty || request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }
        }
    }
}
