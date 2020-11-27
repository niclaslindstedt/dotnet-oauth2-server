using Etimo.Id.Entities;
using Etimo.Id.Entities.Abstractions;

namespace Etimo.Id.Abstractions
{
    public interface IJwtTokenFactory
    {
        JwtToken CreateJwtToken(IJwtTokenRequest request);
    }
}
