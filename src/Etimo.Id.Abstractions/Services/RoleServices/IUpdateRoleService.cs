using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IUpdateRoleService
    {
        Task<Role> UpdateAsync(Role updatedRole);
        Task<Role> UpdateAsync(Role updatedRole, Guid userId);
    }
}
