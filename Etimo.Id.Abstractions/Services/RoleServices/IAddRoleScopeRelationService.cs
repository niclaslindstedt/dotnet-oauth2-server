using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAddRoleScopeRelationService
    {
        Task<List<Scope>> AddScopeRelationAsync(Guid roleId, Guid scopeId);
        Task<List<Scope>> AddScopeRelationAsync(Guid roleId, Guid scopeId, Guid userId);
    }
}
