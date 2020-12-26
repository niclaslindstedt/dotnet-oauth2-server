using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class AccessTokenRepository : IAccessTokenRepository
    {
        private readonly IEtimoIdDbContext _dbContext;

        public AccessTokenRepository(IEtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<AccessToken> FindAsync(Guid accessTokenId)
        {
            return _dbContext.AccessTokens.FindAsync(accessTokenId).AsTask();
        }

        public void Add(AccessToken token)
        {
            _dbContext.AccessTokens.Add(token);
        }

        public void Delete(AccessToken token)
        {
            if (token != null)
            {
                _dbContext.AccessTokens.Remove(token);
            }
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
