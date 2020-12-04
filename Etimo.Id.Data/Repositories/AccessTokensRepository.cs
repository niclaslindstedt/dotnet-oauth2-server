using System;
using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class AccessTokensRepository : IAccessTokensRepository
    {
        private readonly IEtimoIdDbContext _dbContext;

        public AccessTokensRepository(IEtimoIdDbContext dbContext)
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

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Delete(AccessToken token)
        {
            if (token != null)
            {
                _dbContext.AccessTokens.Remove(token);
            }
        }
    }
}
