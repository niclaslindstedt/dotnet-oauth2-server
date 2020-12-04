using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IRefreshTokensRepository
    {
        Task<RefreshToken> FindAsync(string refreshToken);
        void Add(RefreshToken refreshToken);
        Task<int> SaveAsync();
    }
}
