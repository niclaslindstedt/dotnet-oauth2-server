using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface ITokenGenerator
    {
        Task<JwtToken> GenerateTokenAsync(TokenRequest request);
    }
}
