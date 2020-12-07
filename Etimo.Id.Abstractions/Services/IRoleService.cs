using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllAsync();
        Task<List<Role>> GetByUserIdAsync(Guid userId);
        Task<Role> FindAsync(Guid roleId);
        Task<Role> FindAsync(Guid roleId, Guid userId);
        Task<Role> AddAsync(Role role);
        Task<Role> AddAsync(Role role, Guid userId);
        Task<Role> UpdateAsync(Role updatedRole);
        Task<Role> UpdateAsync(Role updatedRole, Guid userId);
        Task DeleteAsync(Guid roleId);
        Task DeleteAsync(Guid roleId, Guid userId);
    }
}
