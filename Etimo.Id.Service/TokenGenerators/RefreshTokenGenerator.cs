using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace Etimo.Id.Service.TokenGenerators
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly IApplicationService _applicationService;
        private readonly IRefreshTokensRepository _refreshTokensRepository;
        private readonly IAccessTokensRepository _accessTokensRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly OAuth2Settings _settings;

        private IRefreshTokenRequest _request;
        private RefreshToken _refreshToken;
        private string _scope;

        public RefreshTokenGenerator(
            IApplicationService applicationService,
            IRefreshTokensRepository refreshTokensRepository,
            IAccessTokensRepository accessTokensRepository,
            IJwtTokenFactory jwtTokenFactory,
            IPasswordGenerator passwordGenerator,
            OAuth2Settings settings)
        {
            _applicationService = applicationService;
            _refreshTokensRepository = refreshTokensRepository;
            _accessTokensRepository = accessTokensRepository;
            _jwtTokenFactory = jwtTokenFactory;
            _passwordGenerator = passwordGenerator;
            _settings = settings;
        }

        public async Task<JwtToken> GenerateTokenAsync(IRefreshTokenRequest request)
        {
            await ValidateRequestAsync(request);
            var refreshToken = GenerateRefreshToken();
            var jwtToken = CreateJwtToken();

            jwtToken.RefreshToken = refreshToken.RefreshTokenId;
            refreshToken.AccessTokenId = jwtToken.TokenId;
            refreshToken.Code = _refreshToken.Code;
            _refreshToken.Used = true;
            var accessToken = jwtToken.ToAccessToken();
            _accessTokensRepository.Add(accessToken);

            await SaveAsync();

            return jwtToken;
        }

        public RefreshToken GenerateRefreshToken(int applicationId, string redirectUri, Guid userId, string scope = null)
        {
            var refreshToken = new RefreshToken
            {
                RefreshTokenId = _passwordGenerator.Generate(_settings.RefreshTokenLength),
                ApplicationId = applicationId,
                ExpirationDate = DateTime.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays),
                RedirectUri = redirectUri,
                UserId = userId,
                Scope = scope
            };

            _refreshTokensRepository.Add(refreshToken);

            return refreshToken;
        }

        private async Task ValidateRequestAsync(IRefreshTokenRequest request)
        {
            _request = request;

            if (_request.ClientId == Guid.Empty || _request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            if (_request.RefreshToken == null)
            {
                throw new InvalidGrantException("Refresh token could not be found");
            }

            _refreshToken = await _refreshTokensRepository.FindAsync(request.RefreshToken);
            if (_refreshToken == null || _refreshToken.IsExpired || _refreshToken.Application.ClientId != request.ClientId)
            {
                throw new InvalidGrantException("Refresh token could not be found.");
            }

            // If someone tries to use the same refresh token twice, disable the access token.
            if (_refreshToken.Used)
            {
                if (_refreshToken.AccessToken != null && !_refreshToken.AccessToken.IsExpired)
                {
                    _refreshToken.AccessToken.Disabled = true;
                    await _accessTokensRepository.SaveAsync();
                }

                throw new InvalidGrantException("Refresh token could not be found.");
            }

            await _applicationService.AuthenticateAsync(_request.ClientId, _request.ClientSecret);

            // Make sure all requested scopes were requested in the original refresh token.
            if (request.Scope != null)
            {
                var scopes = request.Scope.Split(" ");
                foreach (var scope in scopes)
                {
                    // Only allow the refreshed token to use the scopes issued with the
                    // original token as per https://tools.ietf.org/html/rfc6749#section-6
                    var originalScopes = _refreshToken.AuthorizationCode.Scope.Split(" ");
                    if (originalScopes.All(scopeName => scopeName != scope))
                    {
                        throw new InvalidScopeException("The provided scope is invalid.");
                    }
                }
            }

            _scope = request.Scope ?? _refreshToken.Scope;
        }

        private RefreshToken GenerateRefreshToken()
        {
            return GenerateRefreshToken(_refreshToken.ApplicationId, _refreshToken.RedirectUri, _refreshToken.UserId, _scope);
        }

        private JwtToken CreateJwtToken()
        {
            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { _refreshToken.Application.ClientId.ToString() },
                Subject = _refreshToken.UserId.ToString(),
                Scope = _scope
            };

            return _jwtTokenFactory.CreateJwtToken(jwtRequest);
        }

        private async Task SaveAsync()
        {
            await _refreshTokensRepository.SaveAsync();
            await _accessTokensRepository.SaveAsync();
        }
    }
}
