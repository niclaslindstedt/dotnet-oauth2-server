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
        private readonly IEtimoIdDbContext _dbContext;

        public ApplicationsRepository(IEtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Application>> GetAllAsync()
        {
            return _dbContext.Applications.ToListAsync();
        }

        public Task<List<Application>> GetByUserIdAsync(Guid userId)
        {
            return _dbContext.Applications.Where(a => a.UserId == userId).ToListAsync();
        }

        public Task<Application> FindAsync(int applicationId)
        {
            return _dbContext.Applications.FindAsync(applicationId).AsTask();
        }

        public Task<Application> FindAsync(Guid clientId)
        {
            return _dbContext.Applications.FirstOrDefaultAsync(a => a.ClientId == clientId);
        }

        public Task<Application> FindByClientIdAsync(Guid clientId)
        {
            return _dbContext.Applications.FirstOrDefaultAsync(a => a.ClientId == clientId);
        }

        public void Add(Application application)
        {
            _dbContext.Applications.Add(application);
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
