using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IApplicationsRepository
    {
        Task<List<Application>> GetAllAsync();
        Task<List<Application>> GetByUserIdAsync(Guid userId);
        ValueTask<Application> FindAsync(int applicationId);
        Task<Application> FindAsync(Guid clientId);
        Task<Application> FindByClientIdAsync(Guid clientId);
        Application Add(Application application);
        Task<int> SaveAsync();
        void Delete(Application application);
    }
}
