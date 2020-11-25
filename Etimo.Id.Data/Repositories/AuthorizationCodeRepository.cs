using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
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

        public void Add(AuthorizationCode code)
        {
            _dbContext.AuthorizationCodes.Add(code);
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void RemoveRange(List<AuthorizationCode> codes)
        {
            _dbContext.AuthorizationCodes.RemoveRange(codes);
        }
    }
}
