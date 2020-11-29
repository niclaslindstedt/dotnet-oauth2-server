using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
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

        public ValueTask<AuthorizationCode> FindAsync(string code)
        {
            return _dbContext.AuthorizationCodes.FindAsync(code);
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
