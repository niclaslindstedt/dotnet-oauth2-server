using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAddApplicationService
    {
        Task<Application> AddAsync(Application application, Guid userId);
    }
}
