using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthorizationCodeRepository
    {
        void Add(AuthorizationCode code);
        ValueTask<AuthorizationCode> FindAsync(string code);
        void Remove(AuthorizationCode code);
        Task<int> SaveAsync();
    }
}
