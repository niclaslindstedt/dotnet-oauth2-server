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
        private readonly IUserService _userService;
        private readonly IAuthenticateClientService _authenticateClientService;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        private IResourceOwnerPasswordCredentialsTokenRequest _request;
        private User _user;
        private Application _application;

        public ResourceOwnerCredentialsTokenGenerator(
            IUserService userService,
            IAuthenticateClientService applicationService,
            IAccessTokenRepository accessTokenRepository,
            IJwtTokenFactory jwtTokenFactory)
        {
            _userService = userService;
            _authenticateClientService = applicationService;
            _accessTokenRepository = accessTokenRepository;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(IResourceOwnerPasswordCredentialsTokenRequest request)
        {
            await ValidateRequestAsync(request);
            var jwtToken = CreateJwtToken();
            var accessToken = jwtToken.ToAccessToken();
            _accessTokenRepository.Add(accessToken);
            await _accessTokenRepository.SaveAsync();

            return jwtToken;
        }

        private async Task ValidateRequestAsync(IResourceOwnerPasswordCredentialsTokenRequest request)
        {
            _request = request;

            if (_request.ClientId == Guid.Empty || _request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            if (_request.Username == null || _request.Password == null)
            {
                throw new InvalidGrantException("Invalid resource owner credentials.");
            }

            _user = await _userService.AuthenticateAsync(request.Username, request.Password);
            _application = await _authenticateClientService.AuthenticateAsync(request.ClientId, request.ClientSecret);
        }

        private JwtToken CreateJwtToken()
        {
            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { _application.ClientId.ToString() },
                Subject = _user.UserId.ToString()
            };

            return _jwtTokenFactory.CreateJwtToken(jwtRequest);
        }
    }
}
