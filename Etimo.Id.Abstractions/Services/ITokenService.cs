using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface ITokenService
    {
        Task<JwtToken> GenerateTokenAsync(ITokenRequest request);
    }
}
