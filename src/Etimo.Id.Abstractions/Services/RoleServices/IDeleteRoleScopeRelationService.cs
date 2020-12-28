using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IDeleteRoleScopeRelationService
    {
        Task<List<Scope>> DeleteScopeRelationAsync(Guid roleId, Guid scopeId);

        Task<List<Scope>> DeleteScopeRelationAsync(
            Guid roleId,
            Guid scopeId,
            Guid userId);
    }
}
