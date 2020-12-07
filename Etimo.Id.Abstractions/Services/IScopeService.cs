using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IScopeService
    {
        Task<List<Scope>> GetAllAsync();
        Task<List<Scope>> GetByClientIdAsync(Guid clientId);
        Task<Scope> FindAsync(Guid scopeId);
        Task<Scope> FindAsync(Guid scopeId, Guid userId);
        Task<Scope> AddAsync(Scope scope);
        Task<Scope> AddAsync(Scope scope, Guid userId);
        Task<Scope> UpdateAsync(Scope updatedScope);
        Task<Scope> UpdateAsync(Scope updatedScope, Guid userId);
        Task DeleteAsync(Guid scopeId);
        Task DeleteAsync(Guid scopeId, Guid userId);
    }
}
