using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class ValidateTokenService : IValidateTokenService
    {
        private readonly IAccessTokenRepository _accessTokenRepository;

        public ValidateTokenService(IAccessTokenRepository accessTokenRepository)
        {
            _accessTokenRepository = accessTokenRepository;
        }

        public async Task ValidateTokenAsync(Guid accessTokenId)
        {
            AccessToken accessToken = await _accessTokenRepository.FindAsync(accessTokenId);
            if (accessToken == null || accessToken.Disabled)
            {
                throw new UnauthorizedException("Access token has been disabled, please authenticate again.");
            }
        }
    }
}
