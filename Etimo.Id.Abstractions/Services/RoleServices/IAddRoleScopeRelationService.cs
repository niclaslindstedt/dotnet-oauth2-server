using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAddRoleScopeRelationService
    {
        Task<Role> AddScopeRelationAsync(Guid roleId, Guid scopeId);
        Task<Role> AddScopeRelationAsync(Guid roleId, Guid scopeId, Guid userId);
    }
}
