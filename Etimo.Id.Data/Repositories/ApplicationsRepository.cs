using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class ApplicationsRepository : IApplicationsRepository
    {
        private readonly EtimoIdDbContext _dbContext;

        public ApplicationsRepository(EtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Application>> GetAllAsync()
        {
            return _dbContext.Applications.ToListAsync();
        }

        public Task<List<Application>> GetByUserIdAsync(Guid userId)
        {
            return _dbContext.Applications.Where(c => c.UserId == userId).ToListAsync();
        }

        public ValueTask<Application> FindAsync(int applicationId)
        {
            return _dbContext.Applications.FindAsync(applicationId);
        }

        public Task<Application> FindAsync(Guid clientId)
        {
            return _dbContext.Applications.FirstOrDefaultAsync(c => c.ClientId == clientId);
        }

        public Application Add(Application application)
        {
            return _dbContext.Applications.Add(application).Entity;
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Delete(Application application)
        {
            if (application != null)
            {
                _dbContext.Applications.Remove(application);
            }
        }
    }
}
