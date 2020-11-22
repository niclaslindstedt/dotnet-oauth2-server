using Etimo.Id.Entities;

namespace Etimo.Id.Abstractions
{
    public interface ITokenGeneratorFactory
    {
        ITokenGenerator Create(TokenRequest request);
    }
}
