using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;
using Etimo.Id.Service.Exceptions;

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

        public async Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            var code = await _authorizationCodeRepository.FindByCodeAsync(request.Code);
            if (code == null || code.IsExpired || !code.Authorized || code.UserId == null)
            {
                throw new InvalidGrantException("Invalid authorization code.");
            }

            var application = await _applicationsService.AuthenticateAsync(new Guid(request.ClientId), request.ClientSecret);
            if (request.RedirectUri != null && application.RedirectUri != request.RedirectUri)
            {
                throw new InvalidClientException("The provided redirect URI does not match the one on record.");
            }

            var refreshToken = new RefreshToken
            {
                ApplicationId = application.ApplicationId,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UserId = code.UserId.Value
            };
            var oldRefreshTokens = await _refreshTokensRepository.GetByUserIdAsync(application.UserId);
            _refreshTokensRepository.RemoveRange(oldRefreshTokens);
            _refreshTokensRepository.Add(refreshToken);

            request.ClientId = refreshToken.UserId.ToString();
            var jwtToken = _jwtTokenFactory.CreateJwtToken(request);
            jwtToken.RefreshToken = refreshToken.RefreshTokenId.ToString();

            await _refreshTokensRepository.SaveAsync();

            return jwtToken;
        }
    }
}
