using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthorizationCodeTokenGenerator
    {
        Task<JwtToken> GenerateTokenAsync(IAuthorizationCodeRequest request);
    }
}
