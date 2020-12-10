using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IDeleteRoleService
    {
        Task DeleteAsync(Guid roleId);
        Task DeleteAsync(Guid roleId, Guid userId);
    }
}
