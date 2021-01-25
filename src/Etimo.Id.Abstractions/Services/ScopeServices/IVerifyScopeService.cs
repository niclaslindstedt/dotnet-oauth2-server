using Etimo.Id.Entities;

namespace Etimo.Id.Abstractions
{
    public interface IVerifyScopeService
    {
        bool Verify(string scope, User user);
    }
}
