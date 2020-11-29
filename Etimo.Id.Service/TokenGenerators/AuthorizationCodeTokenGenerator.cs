using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class AuthorizationCodeTokenGenerator : IAuthorizationCodeTokenGenerator
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IRefreshTokensRepository _refreshTokensRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public AuthorizationCodeTokenGenerator(
            IApplicationsService applicationsService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IRefreshTokensRepository refreshTokensRepository,
            IJwtTokenFactory jwtTokenFactory)
        {
            _applicationsService = applicationsService;
            _refreshTokenGenerator = refreshTokenGenerator;
            _authorizationCodeRepository = authorizationCodeRepository;
            _refreshTokensRepository = refreshTokensRepository;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(IAuthorizationCodeRequest request)
        {
            ValidateRequest(request);

            var code = await _authorizationCodeRepository.FindAsync(request.Code);
            if (code?.UserId == null || code.IsExpired || code.Used)
            {
                throw new InvalidGrantException("Invalid authorization code.");
            }

            var application = await _applicationsService.AuthenticateAsync(request.ClientId, request.ClientSecret);
            var redirectUri = request.RedirectUri ?? application.RedirectUri;
            if (redirectUri != application.RedirectUri)
            {
                throw new InvalidGrantException("The provided redirect URI does not match the one on record.");
            }

            var refreshToken = _refreshTokenGenerator.GenerateRefreshToken(
                application.ApplicationId, redirectUri, code.UserId.GetValueOrDefault());

            var jwtRequest = new JwtTokenRequest
            {
                Subject = code.UserId?.ToString(),
                Audience = new List<string> { application.HomepageUri, redirectUri }
            };

            var jwtToken = _jwtTokenFactory.CreateJwtToken(jwtRequest);
            jwtToken.RefreshToken = refreshToken.RefreshTokenId;

            code.Used = true;

            await _authorizationCodeRepository.SaveAsync();
            await _refreshTokensRepository.SaveAsync();

            return jwtToken;
        }

        private static void ValidateRequest(IAuthorizationCodeRequest request)
        {
            if (request.ClientId == Guid.Empty || request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            if (request.Code == null)
            {
                throw new InvalidGrantException("Invalid authorization code.");
            }
        }
    }
}
