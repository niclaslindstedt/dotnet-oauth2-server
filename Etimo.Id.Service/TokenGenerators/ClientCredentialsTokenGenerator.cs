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

        private IClientCredentialsTokenRequest _request;
        private Application _application;

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
            await ValidateRequestAsync(request);
            var jwtToken = CreateJwtToken();
            var accessToken = jwtToken.ToAccessToken();
            _accessTokensRepository.Add(accessToken);
            await _accessTokensRepository.SaveAsync();

            return jwtToken;
        }

        private async Task ValidateRequestAsync(IClientCredentialsTokenRequest request)
        {
            _request = request;

            if (_request.ClientId == Guid.Empty || _request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            _application = await _applicationService.AuthenticateAsync(_request.ClientId, _request.ClientSecret);
            if (_application.Type == ClientTypes.Public)
            {
                throw new UnauthorizedClientException("Public clients cannot use the client credentials grant.");
            }
        }

        private JwtToken CreateJwtToken()
        {
            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { _request.ClientId.ToString() },
                Subject = _application.UserId.ToString()
            };

            return _jwtTokenFactory.CreateJwtToken(jwtRequest);
        }
    }
}
