using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IGetRolesService
    {
        Task<List<Role>> GetAllAsync();
        Task<List<Role>> GetByUserIdAsync(Guid userId);
        Task<List<Role>> GetByApplicationId(int applicationId);
        Task<List<Role>> GetByApplicationId(int applicationId, Guid userId);
    }
}
