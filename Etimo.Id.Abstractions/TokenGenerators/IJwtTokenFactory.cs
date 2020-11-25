using Etimo.Id.Entities;

namespace Etimo.Id.Abstractions
{
    public interface IJwtTokenFactory
    {
        JwtToken CreateJwtToken(TokenRequest request);
    }
}
