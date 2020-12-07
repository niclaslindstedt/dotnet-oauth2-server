using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class AuthorizationCodeRepository : IAuthorizationCodeRepository
    {
        private readonly IEtimoIdDbContext _dbContext;

        public AuthorizationCodeRepository(IEtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<AuthorizationCode> FindAsync(string code)
        {
            return _dbContext.AuthorizationCodes.FindAsync(code).AsTask();
        }

        public void Add(AuthorizationCode code)
        {
            _dbContext.AuthorizationCodes.Add(code);
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
