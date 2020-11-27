using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IRefreshTokenGenerator
    {
        Task<JwtToken> GenerateTokenAsync(TokenRequest request);
    }
}
