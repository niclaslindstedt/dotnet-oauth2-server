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
        private readonly IAccessTokenRepository         _accessTokenRepository;
        private readonly IAuthenticateClientService     _authenticateClientService;
        private readonly IJwtTokenFactory               _jwtTokenFactory;
        private readonly IRefreshTokenGenerator         _refreshTokenGenerator;
        private readonly IRequestContext                _requestContext;
        private          Application                    _application;
        private          JwtToken                       _jwtToken;
        private          RefreshToken                   _refreshToken;
        private          IClientCredentialsTokenRequest _request;

        public ClientCredentialsTokenGenerator(
            IAuthenticateClientService applicationService,
            IAccessTokenRepository accessTokenRepository,
            IJwtTokenFactory jwtTokenFactory,
            IRefreshTokenGenerator refreshTokenGenerator,
            IRequestContext requestContext)
        {
            _authenticateClientService = applicationService;
            _accessTokenRepository     = accessTokenRepository;
            _jwtTokenFactory           = jwtTokenFactory;
            _refreshTokenGenerator     = refreshTokenGenerator;
            _requestContext            = requestContext;
        }

        public async Task<JwtToken> GenerateTokenAsync(IClientCredentialsTokenRequest request)
        {
            _request = request;

            UpdateContext();
            await ValidateRequestAsync();
            await CreateJwtTokenAsync();
            await GenerateRefreshTokenAsync();

            var accessToken = _jwtToken.ToAccessToken();
            _accessTokenRepository.Add(accessToken);
            await _accessTokenRepository.SaveAsync();

            return _jwtToken;
        }

        private void UpdateContext()
            => _requestContext.ClientId = _request.ClientId;

        private async Task ValidateRequestAsync()
        {
            if (_request.ClientId == Guid.Empty || _request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            _application = await _authenticateClientService.AuthenticateAsync(_request.ClientId, _request.ClientSecret);
            if (_application.Type == ClientTypes.Public)
            {
                throw new UnauthorizedClientException("Public clients cannot use the client credentials grant.");
            }

            if (!_application.AllowClientCredentialsGrant)
            {
                throw new UnsupportedGrantTypeException("This authorization grant is not allowed for this application.");
            }

            if (!_application.AllowCredentialsInBody && _request.CredentialsInBody)
            {
                throw new InvalidGrantException("This application does not allow passing credentials in the request body.");
            }
        }

        private async Task CreateJwtTokenAsync()
        {
            var jwtRequest = new JwtTokenRequest
            {
                Audience        = new List<string> { _request.ClientId.ToString() },
                Subject         = _application.UserId.ToString(),
                LifetimeMinutes = _application.AccessTokenLifetimeMinutes,
            };

            _jwtToken = await _jwtTokenFactory.CreateJwtTokenAsync(jwtRequest);
        }

        private async Task GenerateRefreshTokenAsync()
        {
            if (!_application.GenerateRefreshTokenForClientCredentials) { return; }

            _refreshToken = await _refreshTokenGenerator.GenerateRefreshTokenAsync(_application.ApplicationId, null, _application.UserId);
            _refreshToken.GrantType = GrantTypes.ClientCredentials;
            _refreshToken.AccessTokenId = _jwtToken.TokenId;
            _jwtToken.RefreshToken = _refreshToken.RefreshTokenId;
        }
    }
}
