using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class RefreshTokensRepository : IRefreshTokensRepository
    {
        private readonly EtimoIdDbContext _dbContext;

        public RefreshTokensRepository(EtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ValueTask<RefreshToken> FindAsync(string refreshToken)
        {
            return _dbContext.RefreshTokens.FindAsync(refreshToken);
        }

        public void Add(RefreshToken refreshToken)
        {
            _dbContext.RefreshTokens.Add(refreshToken);
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public Task<List<RefreshToken>> GetByUserIdAsync(Guid userId)
        {
            return _dbContext.RefreshTokens.Where(r => r.UserId == userId).ToListAsync();
        }

        public void RemoveRange(List<RefreshToken> refreshTokens)
        {
           _dbContext.RefreshTokens.RemoveRange(refreshTokens);
        }
    }
}
