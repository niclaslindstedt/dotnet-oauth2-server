using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IDeleteScopeService
    {
        Task DeleteAsync(Guid scopeId);
        Task DeleteAsync(Guid scopeId, Guid userId);
    }
}
