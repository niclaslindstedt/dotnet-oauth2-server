using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class RefreshTokenRepository : IRefreshTokensRepository
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
