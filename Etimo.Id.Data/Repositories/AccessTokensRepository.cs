using System;
using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class AccessTokensRepository : IAccessTokensRepository
    {
        private readonly EtimoIdDbContext _dbContext;

        public AccessTokensRepository(EtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ValueTask<AccessToken> FindAsync(Guid accessTokenId)
        {
            return _dbContext.AccessTokens.FindAsync(accessTokenId);
        }

        public AccessToken Add(AccessToken token)
        {
            return _dbContext.AccessTokens.Add(token).Entity;
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
