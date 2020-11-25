using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class OAuthService : IOAuthService
    {
        private readonly IClientCredentialsTokenGenerator _clientCredentialsTokenGenerator;
        private readonly IPasswordTokenGenerator _passwordTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;

        public OAuthService(
            IClientCredentialsTokenGenerator clientCredentialsTokenGenerator,
            IPasswordTokenGenerator passwordTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator)
        {
            _clientCredentialsTokenGenerator = clientCredentialsTokenGenerator;
            _passwordTokenGenerator = passwordTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
        }

        public Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            ITokenGenerator generator = request.GrantType switch
            {
                GrantTypes.ClientCredentials => _clientCredentialsTokenGenerator,
                GrantTypes.Password => _passwordTokenGenerator,
                GrantTypes.RefreshToken => _refreshTokenGenerator,
                _ => throw new InvalidRequestException("Grant type not supported.")
            };

            return generator.GenerateTokenAsync(request);
        }
    }
}
