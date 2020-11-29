using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IOAuthService
    {
        Task<JwtToken> GenerateTokenAsync(TokenRequest request);
        Task<string> AuthorizeAsync(AuthorizationRequest request);
    }
}
