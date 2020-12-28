using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly IAccessTokenRepository     _accessTokenRepository;
        private readonly IAuthenticateClientService _authenticateClientService;
        private readonly IJwtTokenFactory           _jwtTokenFactory;
        private readonly IPasswordGenerator         _passwordGenerator;
        private readonly IRefreshTokenRepository    _refreshTokenRepository;
        private readonly IRequestContext            _requestContext;
        private readonly OAuth2Settings             _settings;
        private          RefreshToken               _refreshToken;

        private IRefreshTokenRequest _request;
        private string               _scope;

        public RefreshTokenGenerator(
            IAuthenticateClientService applicationService,
            IRefreshTokenRepository refreshTokenRepository,
            IAccessTokenRepository accessTokenRepository,
            IJwtTokenFactory jwtTokenFactory,
            IRequestContext requestContext,
            IPasswordGenerator passwordGenerator,
            OAuth2Settings settings)
        {
            _authenticateClientService = applicationService;
            _refreshTokenRepository    = refreshTokenRepository;
            _accessTokenRepository     = accessTokenRepository;
            _jwtTokenFactory           = jwtTokenFactory;
            _requestContext            = requestContext;
            _passwordGenerator         = passwordGenerator;
            _settings                  = settings;
        }

        public async Task<JwtToken> GenerateTokenAsync(IRefreshTokenRequest request)
        {
            _request = request;

            UpdateContext();
            await ValidateRequestAsync();
            RefreshToken refreshToken = GenerateRefreshToken();
            JwtToken     jwtToken     = await CreateJwtTokenAsync();

            jwtToken.RefreshToken      = refreshToken.RefreshTokenId;
            refreshToken.AccessTokenId = jwtToken.TokenId;
            refreshToken.Code          = _refreshToken.Code;
            _refreshToken.Used         = true;
            var accessToken = jwtToken.ToAccessToken();
            _accessTokenRepository.Add(accessToken);

            await SaveAsync();

            return jwtToken;
        }

        public RefreshToken GenerateRefreshToken(
            int applicationId,
            string redirectUri,
            Guid userId,
            string scope = null)
        {
            var refreshToken = new RefreshToken
            {
                RefreshTokenId = _passwordGenerator.Generate(_settings.RefreshTokenLength),
                ApplicationId  = applicationId,
                ExpirationDate = DateTime.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays),
                RedirectUri    = redirectUri,
                UserId         = userId,
                Scope          = scope,
            };

            _refreshTokenRepository.Add(refreshToken);

            return refreshToken;
        }

        private void UpdateContext()
            => _requestContext.ClientId = _request.ClientId;

        private async Task ValidateRequestAsync()
        {
            if (_request.ClientId == Guid.Empty || _request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            if (_request.RefreshToken == null) { throw new InvalidGrantException("Refresh token could not be found"); }

            _refreshToken = await _refreshTokenRepository.FindAsync(_request.RefreshToken);
            if (_refreshToken == null || _refreshToken.IsExpired || _refreshToken.Application.ClientId != _request.ClientId)
            {
                throw new InvalidGrantException("Refresh token could not be found.");
            }

            // If someone tries to use the same refresh token twice, disable the access token.
            if (_refreshToken.Used)
            {
                if (_refreshToken.AccessToken != null && !_refreshToken.AccessToken.IsExpired)
                {
                    _refreshToken.AccessToken.Disabled = true;
                    await _accessTokenRepository.SaveAsync();
                }

                throw new InvalidGrantException("Refresh token could not be found.");
            }

            await _authenticateClientService.AuthenticateAsync(_request.ClientId, _request.ClientSecret);

            // Make sure all requested scopes were requested in the original refresh token.
            if (_request.Scope != null)
            {
                string[] scopes = _request.Scope.Split(" ");
                foreach (string scope in scopes)
                {
                    // Only allow the refreshed token to use the scopes issued with the
                    // original token as per https://tools.ietf.org/html/rfc6749#section-6
                    string[] originalScopes = _refreshToken.AuthorizationCode.Scope.Split(" ");
                    if (originalScopes.All(scopeName => scopeName != scope))
                    {
                        throw new InvalidScopeException("The provided scope is invalid.");
                    }
                }
            }

            _scope = _request.Scope ?? _refreshToken.Scope;
        }

        private RefreshToken GenerateRefreshToken()
            => GenerateRefreshToken(
                _refreshToken.ApplicationId,
                _refreshToken.RedirectUri,
                _refreshToken.UserId,
                _scope);

        private Task<JwtToken> CreateJwtTokenAsync()
        {
            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { _refreshToken.Application.ClientId.ToString() },
                Subject  = _refreshToken.UserId.ToString(),
                Scope    = _scope,
            };

            return _jwtTokenFactory.CreateJwtTokenAsync(jwtRequest);
        }

        private async Task SaveAsync()
        {
            await _refreshTokenRepository.SaveAsync();
            await _accessTokenRepository.SaveAsync();
        }
    }
}
