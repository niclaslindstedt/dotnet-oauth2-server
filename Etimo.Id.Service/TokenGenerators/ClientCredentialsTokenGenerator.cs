using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service.TokenGenerators
{
    public class ClientCredentialsTokenGenerator : IClientCredentialsTokenGenerator
    {
        private readonly IApplicationsService _applicationsService;
        private readonly IRefreshTokensRepository _refreshTokensRepository;
        private readonly IJwtTokenFactory _jwtTokenFactory;

        public ClientCredentialsTokenGenerator(
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
            var application = await _applicationsService.AuthenticateAsync(new Guid(request.ClientId), request.ClientSecret);

            //TODO: Move this to the authenticate flow
            var refreshToken = new RefreshToken
            {
                ApplicationId = application.ApplicationId,
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                UserId = application.UserId // Should be the userId of the user with the token
            };
            var oldRefreshTokens = await _refreshTokensRepository.GetByUserIdAsync(application.UserId);
            _refreshTokensRepository.RemoveRange(oldRefreshTokens);
            _refreshTokensRepository.Add(refreshToken);
            request.ClientId = application.UserId.ToString();

            var jwtToken = _jwtTokenFactory.CreateJwtToken(request);
            jwtToken.RefreshToken = refreshToken.RefreshTokenId.ToString();

            await _refreshTokensRepository.SaveAsync();

            return jwtToken;
        }
    }
}
