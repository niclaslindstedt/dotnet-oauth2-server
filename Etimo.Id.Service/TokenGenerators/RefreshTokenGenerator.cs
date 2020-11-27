using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IRefreshTokensRepository _refreshTokensRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public RefreshTokenGenerator(
            IApplicationsService applicationsService,
            IRefreshTokensRepository refreshTokensRepository,
            IJwtTokenFactory jwtTokenFactory)
        {
            _applicationsService = applicationsService;
            _refreshTokensRepository = refreshTokensRepository;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            ValidateRequest(request);

            var clientId = new Guid(request.ClientId);
            var refreshToken = await _refreshTokensRepository.FindAsync(request.RefreshToken.Value);
            if (refreshToken == null || refreshToken.IsExpired || refreshToken.Application.ClientId != clientId)
            {
                throw new InvalidGrantException("Refresh token could not be found.");
            }

            await _applicationsService.AuthenticateAsync(clientId, request.ClientSecret);

            await RecycleRefreshTokensAsync(refreshToken);
            refreshToken = GenerateRefreshToken(refreshToken.ApplicationId, refreshToken.RedirectUri, refreshToken.UserId);

            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { refreshToken.Application.HomepageUri, refreshToken.RedirectUri },
                Subject = refreshToken.UserId.ToString()
            };

            var response = _jwtTokenFactory.CreateJwtToken(jwtRequest);
            response.RefreshToken = refreshToken.RefreshTokenId.ToString();

            await _refreshTokensRepository.SaveAsync();

            return response;
        }

        public RefreshToken GenerateRefreshToken(int applicationId, string redirectUri, Guid userId)
        {
            var refreshToken = new RefreshToken
            {
                ApplicationId = applicationId,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                RedirectUri = redirectUri,
                UserId = userId
            };

            _refreshTokensRepository.Add(refreshToken);

            return refreshToken;
        }

        public async Task RecycleRefreshTokensAsync(RefreshToken refreshToken)
        {
            var oldRefreshTokens = await _refreshTokensRepository.GetByUserIdAsync(refreshToken.UserId);

            // Add this refresh token to the list of old refresh tokens.
            oldRefreshTokens.Add(refreshToken);

            _refreshTokensRepository.RemoveRange(oldRefreshTokens);
        }

        private static void ValidateRequest(TokenRequest request)
        {
            if (request.ClientId == null || request.ClientSecret == null)
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
