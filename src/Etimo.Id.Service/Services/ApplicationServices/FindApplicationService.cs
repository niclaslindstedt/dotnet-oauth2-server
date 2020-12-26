using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class FindApplicationService : IFindApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;

        public FindApplicationService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<Application> FindAsync(int applicationId)
        {
            var application = await _applicationRepository.FindAsync(applicationId);
            if (application == null)
            {
                throw new NotFoundException();
            }

            return application;
        }

        public async Task<Application> FindAsync(int applicationId, Guid userId)
        {
            var application = await _applicationRepository.FindAsync(applicationId);
            if (application?.UserId != userId)
            {
                throw new NotFoundException();
            }

            return application;
        }

        public Task<Application> FindByClientIdAsync(Guid clientId)
        {
            return _applicationRepository.FindByClientIdAsync(clientId);
        }
    }
}
