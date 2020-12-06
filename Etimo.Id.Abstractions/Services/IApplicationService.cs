using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IApplicationService
    {
        Task<List<Application>> GetAllAsync();
        Task<List<Application>> GetByUserIdAsync(Guid userId);
        Task<Application> AddAsync(Application application, Guid userId);
        Task<Application> UpdateAsync(Application application, Guid userId);
        ValueTask<Application> FindAsync(int applicationId);
        Task<Application> FindAsync(int applicationId, Guid userId);
        Task<Application> FindByClientIdAsync(Guid clientId);
        Task DeleteAsync(int applicationId);
        Task DeleteAsync(int applicationId, Guid userId);
        Task<Application> AuthenticateAsync(Guid clientId, string clientSecret);
        Task<Application> GenerateSecretAsync(int applicationId, Guid userId);
    }
}
