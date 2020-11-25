using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
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

            request.UserId = refreshToken.UserId;

            return _jwtTokenFactory.CreateJwtToken(request);
        }

        private static void ValidateRequest(TokenRequest request)
        {
            if (request.RefreshToken == null || request.ClientId == null || request.ClientSecret == null)
            {
                throw new InvalidGrantException("Refresh token could not be found");
            }
        }
    }
}
