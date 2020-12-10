using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAddScopeService
    {
        Task<Scope> AddAsync(Scope scope);
        Task<Scope> AddAsync(Scope scope, Guid userId);
    }
}
