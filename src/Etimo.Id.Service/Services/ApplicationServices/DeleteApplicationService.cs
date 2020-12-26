using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class DeleteApplicationService : IDeleteApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;

        public DeleteApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task DeleteAsync(int applicationId)
        {
            var application = await _applicationRepository.FindAsync(applicationId);
            await DeleteAsync(application);
        }

        public async Task DeleteAsync(int applicationId, Guid userId)
        {
            var application = await _applicationRepository.FindAsync(applicationId);
            if (application?.UserId == userId)
            {
                await DeleteAsync(application);
            }
        }

        private async Task DeleteAsync(Application application)
        {
            if (application != null)
            {
                _applicationRepository.Delete(application);
                await _applicationRepository.SaveAsync();
            }
        }
    }
}
