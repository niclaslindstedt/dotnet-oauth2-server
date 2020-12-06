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
    public class AuthorizationCodeTokenGenerator : IAuthorizationCodeTokenGenerator
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IAuthorizationCodeRepository _authorizationCodeRepository;
        private readonly IAccessTokensRepository _accessTokensRepository;
        private readonly IRefreshTokensRepository _refreshTokensRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public AuthorizationCodeTokenGenerator(
            IApplicationsService applicationsService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IAuthorizationCodeRepository authorizationCodeRepository,
            IAccessTokensRepository accessTokensRepository,
            IRefreshTokensRepository refreshTokensRepository,
            IJwtTokenFactory jwtTokenFactory)
        {
            _applicationsService = applicationsService;
            _refreshTokenGenerator = refreshTokenGenerator;
            _authorizationCodeRepository = authorizationCodeRepository;
            _accessTokensRepository = accessTokensRepository;
            _refreshTokensRepository = refreshTokensRepository;
            _jwtTokenFactory = jwtTokenFactory;
        }

        public async Task<JwtToken> GenerateTokenAsync(IAuthorizationCodeTokenRequest request)
        {
            ValidateRequest(request);

            var code = await _authorizationCodeRepository.FindAsync(request.Code);
            if (code?.UserId == null || code.IsExpired)
            {
                throw new InvalidGrantException("Invalid authorization code.");
            }

            // If someone tries to use the same authorization code twice, disable the access token.
            if (code.Used)
            {
                if (code.AccessToken != null)
                {
                    code.AccessToken.Disabled = true;
                    await _accessTokensRepository.SaveAsync();
                }

                throw new InvalidGrantException("Invalid authorization code.");
            }

            if (code.ClientId != request.ClientId)
            {
                throw new InvalidGrantException("Invalid client id.");
            }

            var application = await _applicationsService.FindByClientIdAsync(request.ClientId);
            if (application.Type == ClientTypes.Confidential)
            {
                await _applicationsService.AuthenticateAsync(request.ClientId, request.ClientSecret);
            }

            var redirectUri = request.RedirectUri ?? application.RedirectUri;
            if (redirectUri != application.RedirectUri)
            {
                throw new InvalidGrantException("The provided redirect URI does not match the one on record.");
            }

            var jwtRequest = new JwtTokenRequest
            {
                Audience = new List<string> { code.ClientId.ToString() },
                Subject = code.UserId?.ToString()
            };

            var jwtToken = _jwtTokenFactory.CreateJwtToken(jwtRequest);
            var refreshToken = _refreshTokenGenerator.GenerateRefreshToken(
                application.ApplicationId, redirectUri, code.UserId.GetValueOrDefault());
            refreshToken.AccessTokenId = jwtToken.TokenId;
            jwtToken.RefreshToken = refreshToken.RefreshTokenId;

            code.Used = true;
            code.AccessTokenId = jwtToken.TokenId;

            _accessTokensRepository.Add(jwtToken.ToAccessToken());

            await _authorizationCodeRepository.SaveAsync();
            await _refreshTokensRepository.SaveAsync();
            await _accessTokensRepository.SaveAsync();

            return jwtToken;
        }

        private static void ValidateRequest(IAuthorizationCodeTokenRequest request)
        {
            if (request.ClientId == Guid.Empty)
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
