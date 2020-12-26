using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IScopeRepository
    {
        Task<List<Scope>> GetAllAsync();
        Task<Scope> FindAsync(Guid scopeId);
        void Add(Scope scope);
        void Delete(Scope scope);
        Task<int> SaveAsync();
    }
}
