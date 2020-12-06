using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllAsync();
        Task<Role> FindAsync(Guid roleId);
        Task<Role> FindAsync(Guid roleId, Guid userId);
        Task<Role> AddAsync(Role role, Guid userId);
        Task<Role> UpdateAsync(Role role, Guid userId);
        Task DeleteAsync(Guid roleId);
    }
}
