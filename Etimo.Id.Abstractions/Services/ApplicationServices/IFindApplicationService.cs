using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IFindApplicationService
    {
        Task<Application> FindAsync(int applicationId);
        Task<Application> FindAsync(int applicationId, Guid userId);
        Task<Application> FindByClientIdAsync(Guid clientId);
    }
}
