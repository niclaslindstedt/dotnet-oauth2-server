using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IDeleteApplicationService
    {
        Task DeleteAsync(int applicationId);
        Task DeleteAsync(int applicationId, Guid userId);
    }
}
