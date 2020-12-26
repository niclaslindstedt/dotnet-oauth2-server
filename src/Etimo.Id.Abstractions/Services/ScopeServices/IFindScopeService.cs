using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IFindScopeService
    {
        Task<Scope> FindAsync(Guid scopeId);
        Task<Scope> FindAsync(Guid scopeId, Guid userId);
    }
}
