using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IEtimoIdDbContext _dbContext;

        public RefreshTokenRepository(IEtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<RefreshToken> FindAsync(string refreshToken)
        {
            return _dbContext.RefreshTokens.FindAsync(refreshToken).AsTask();
        }

        public void Add(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Add(refreshToken);
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
