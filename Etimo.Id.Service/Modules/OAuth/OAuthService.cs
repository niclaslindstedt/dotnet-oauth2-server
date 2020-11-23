using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Service.OAuth
{
    public class OAuthService : IOAuthService
    {
        private readonly ITokenGeneratorFactory _tokenGeneratorFactory;

        public OAuthService(ITokenGeneratorFactory tokenGeneratorFactory)
        {
            _tokenGeneratorFactory = tokenGeneratorFactory;
        }

        public Task<JwtToken> GenerateTokenAsync(TokenRequest request)
        {
            var tokenGenerator = _tokenGeneratorFactory.Create(request);
            return tokenGenerator.GenerateTokenAsync(request);
        }
    }
}
