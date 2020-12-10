using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAddRoleService
    {
        Task<Role> AddAsync(Role role);
        Task<Role> AddAsync(Role role, Guid userId);
    }
}
