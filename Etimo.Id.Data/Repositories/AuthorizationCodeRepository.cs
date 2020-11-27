using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class AuthorizationCodeRepository : IAuthorizationCodeRepository
    {
        private readonly EtimoIdDbContext _dbContext;

        public AuthorizationCodeRepository(EtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ValueTask<AuthorizationCode> FindAsync(Guid codeId)
        {
            return _dbContext.AuthorizationCodes.FindAsync(codeId);
        }

        public Task<AuthorizationCode> FindByCodeAsync(string code)
        {
            return _dbContext.AuthorizationCodes.FirstOrDefaultAsync(ac => ac.Code == code);
        }

        public void Add(AuthorizationCode code)
        {
            _dbContext.AuthorizationCodes.Add(code);
        }

        public void Remove(AuthorizationCode code)
        {
            _dbContext.AuthorizationCodes.Remove(code);
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
