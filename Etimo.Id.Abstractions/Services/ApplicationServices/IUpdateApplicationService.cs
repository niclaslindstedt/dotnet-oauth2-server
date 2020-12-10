using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IUpdateApplicationService
    {
        Task<Application> UpdateAsync(Application updatedApplication, Guid userId);
    }
}
