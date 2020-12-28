using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IFindRoleService
    {
        Task<Role> FindAsync(Guid roleId);
        Task<Role> FindAsync(Guid roleId, Guid userId);
    }
}
