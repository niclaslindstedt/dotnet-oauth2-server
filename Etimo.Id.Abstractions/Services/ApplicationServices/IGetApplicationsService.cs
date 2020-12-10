using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IGetApplicationsService
    {
        Task<List<Application>> GetAllAsync();
        Task<List<Application>> GetByUserIdAsync(Guid userId);
    }
}
