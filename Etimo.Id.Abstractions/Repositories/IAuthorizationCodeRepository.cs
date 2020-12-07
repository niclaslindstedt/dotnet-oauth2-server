using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuthorizationCodeRepository
    {
        Task<AuthorizationCode> FindAsync(string code);
        void Add(AuthorizationCode code);
        Task<int> SaveAsync();
    }
}
