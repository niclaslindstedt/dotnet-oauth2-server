using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Settings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            ValidateRequest(request);

            var refreshToken = await _refreshTokensRepository.FindAsync(request.RefreshToken);
            if (refreshToken == null || refreshToken.IsExpired || refreshToken.Application.ClientId != request.ClientId)
            {
                throw new InvalidGrantException("Refresh token could not be found.");
            }

            // If someone tries to use the same refresh token twice, disable the access token.
            if (refreshToken.Used)
            {
                if (refreshToken.AccessToken != null && !refreshToken.AccessToken.IsExpired)
                {
                    refreshToken.AccessToken.Disabled = true;
                    await _accessTokensRepository.SaveAsync();
                }

                throw new InvalidGrantException("Refresh token could not be found.");
            }

            await _applicationService.AuthenticateAsync(request.ClientId, request.ClientSecret);

            refreshToken.Used = true;
            refreshToken = GenerateRefreshToken(refreshToken.ApplicationId, refreshToken.RedirectUri, refreshToken.UserId);

            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { refreshToken.Application.ClientId.ToString() },
                Subject = refreshToken.UserId.ToString()
            };

            var jwtToken = _jwtTokenFactory.CreateJwtToken(jwtRequest);
            jwtToken.RefreshToken = refreshToken.RefreshTokenId;
            refreshToken.AccessTokenId = jwtToken.TokenId;

            _accessTokensRepository.Add(jwtToken.ToAccessToken());

            await _refreshTokensRepository.SaveAsync();
            await _accessTokensRepository.SaveAsync();

            return jwtToken;
        }

        public RefreshToken GenerateRefreshToken(int applicationId, string redirectUri, Guid userId)
        {
            var refreshToken = new RefreshToken
            {
                RefreshTokenId = _passwordGenerator.Generate(_settings.RefreshTokenLength),
                ApplicationId = applicationId,
                ExpirationDate = DateTime.UtcNow.AddDays(_settings.RefreshTokenLifetimeDays),
                RedirectUri = redirectUri,
                UserId = userId
            };

            _refreshTokensRepository.Add(refreshToken);

            return refreshToken;
        }

        private static void ValidateRequest(IRefreshTokenRequest request)
        {
            if (request.ClientId == Guid.Empty || request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            if (request.RefreshToken == null)
            {
                throw new InvalidGrantException("Refresh token could not be found");
            }
        }
    }
}
