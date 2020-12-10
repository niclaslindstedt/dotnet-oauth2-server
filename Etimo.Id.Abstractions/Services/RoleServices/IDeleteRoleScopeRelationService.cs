using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IDeleteRoleScopeRelationService
    {
        Task<Role> DeleteScopeRelationAsync(Guid roleId, Guid scopeId);
        Task<Role> DeleteScopeRelationAsync(Guid roleId, Guid scopeId, Guid userId);
    }
}
