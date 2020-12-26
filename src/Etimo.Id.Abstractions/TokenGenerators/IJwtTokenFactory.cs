using System.Threading.Tasks;
using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;

namespace Etimo.Id.Abstractions
{
    public interface IJwtTokenFactory
    {
        Task<JwtToken> CreateJwtTokenAsync(IJwtTokenRequest request);
    }
}
