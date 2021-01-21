using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IImplicitTokenGenerator
    {
        Task<JwtToken> GenerateTokenAsync(IImplicitTokenRequest request);
    }
}
