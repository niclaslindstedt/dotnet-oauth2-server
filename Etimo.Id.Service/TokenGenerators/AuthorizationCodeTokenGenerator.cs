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
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IRefreshTokensRepository _refreshTokensRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public AuthorizationCodeTokenGenerator(
            IApplicationsService applicationsService,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IRefreshTokensRepository refreshTokensRepository,
            IJwtTokenFactory jwtTokenFactory)
        {
            _applicationsService = applicationsService;
            _authorizationCodeRepository = authorizationCodeRepository;
            _refreshTokensRepository = refreshTokensRepository;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(IAuthorizationCodeRequest request)
        {
            ValidateRequest(request);

            var code = await _authorizationCodeRepository.FindByCodeAsync(request.Code);
            if (code == null || code.IsExpired || !code.Authorized || code.UserId == null)
            {
                throw new InvalidGrantException("Invalid authorization code.");
            }

            var application = await _applicationsService.AuthenticateAsync(new Guid(request.ClientId), request.ClientSecret);
            var redirectUri = request.RedirectUri ?? application.RedirectUri;
            if (redirectUri != application.RedirectUri)
            {
                throw new InvalidClientException("The provided redirect URI does not match the one on record.");
            }

            var refreshToken = new RefreshToken
            {
                ApplicationId = application.ApplicationId,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                RedirectUri = redirectUri,
                UserId = code.UserId.Value
            };
            var oldRefreshTokens = await _refreshTokensRepository.GetByUserIdAsync(application.UserId);
            _refreshTokensRepository.RemoveRange(oldRefreshTokens);
            _refreshTokensRepository.Add(refreshToken);

            var jwtRequest = new JwtTokenRequest
            {
                Subject = code.UserId?.ToString(),
                Audience = new List<string> { application.HomepageUri, redirectUri }
            };

            var jwtToken = _jwtTokenFactory.CreateJwtToken(jwtRequest);
            jwtToken.RefreshToken = refreshToken.RefreshTokenId.ToString();

            await _refreshTokensRepository.SaveAsync();

            return jwtToken;
        }

        private static void ValidateRequest(IAuthorizationCodeRequest request)
        {
            if (request.ClientId == null || request.ClientSecret == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }

            if (request.Code == null)
            {
                throw new InvalidClientException("Invalid client credentials.");
            }
        }
    }
}
