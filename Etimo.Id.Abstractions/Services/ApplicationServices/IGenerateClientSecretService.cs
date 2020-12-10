using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IGenerateClientSecretService
    {
        Task<Application> GenerateSecretAsync(int applicationId);
        Task<Application> GenerateSecretAsync(int applicationId, Guid userId);
    }
}
