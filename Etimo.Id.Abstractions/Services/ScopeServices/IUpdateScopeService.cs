using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IUpdateScopeService
    {
        Task<Scope> UpdateAsync(Scope updatedScope);
        Task<Scope> UpdateAsync(Scope updatedScope, Guid userId);
    }
}
