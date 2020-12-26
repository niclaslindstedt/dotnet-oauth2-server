using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class ResourceOwnerCredentialsTokenGenerator : IResourceOwnerCredentialsTokenGenerator
    {
        private readonly IAuthenticateUserService _authenticateUserService;
        private readonly IAuthenticateClientService _authenticateClientService;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;
        private readonly IRequestContext _requestContext;

        private IResourceOwnerPasswordCredentialsTokenRequest _request;
        private User _user;
        private Application _application;

        public ResourceOwnerCredentialsTokenGenerator(
            IAuthenticateUserService authenticateUserService,
            IAuthenticateClientService applicationService,
            IAccessTokenRepository accessTokenRepository,
            IJwtTokenFactory jwtTokenFactory,
            IRequestContext requestContext)
        {
            _authenticateUserService = authenticateUserService;
            _authenticateClientService = applicationService;
            _accessTokenRepository = accessTokenRepository;
            _jwtTokenFactory = jwtTokenFactory;
            _requestContext = requestContext;
        }

        public async Task<JwtToken> GenerateTokenAsync(IResourceOwnerPasswordCredentialsTokenRequest request)
        {
            _request = request;

            await ValidateRequestAsync();
            UpdateContext();
            var jwtToken = await CreateJwtTokenAsync();
            var accessToken = jwtToken.ToAccessToken();
            _accessTokenRepository.Add(accessToken);
            await _accessTokenRepository.SaveAsync();

            return jwtToken;
        }

        private void UpdateContext()
        {
            _requestContext.ClientId = _request.ClientId;
            _requestContext.Username = _request.Username;
        }

        private async Task ValidateRequestAsync()
        {

            if (_request.ClientId == Guid.Empty || _request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            if (_request.Username == null || _request.Password == null)
            {
                throw new InvalidGrantException("Invalid resource owner credentials.");
            }

            _user = await _authenticateUserService.AuthenticateAsync(_request.Username, _request.Password);
            _application = await _authenticateClientService.AuthenticateAsync(_request.ClientId, _request.ClientSecret);
        }

        private Task<JwtToken> CreateJwtTokenAsync()
        {
            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { _application.ClientId.ToString() },
                Subject = _user.UserId.ToString()
            };

            return _jwtTokenFactory.CreateJwtTokenAsync(jwtRequest);
        }
    }
}
