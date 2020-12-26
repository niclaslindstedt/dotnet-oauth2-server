using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class UpdateApplicationService : IUpdateApplicationService
    {
        private readonly IFindApplicationService _findApplicationService;
        private readonly IApplicationRepository _applicationRepository;

        public UpdateApplicationService(
            IFindApplicationService findApplicationService,
            IApplicationRepository applicationRepository)
        {
            _findApplicationService = findApplicationService;
            _applicationRepository = applicationRepository;
        }

        public async Task<Application> UpdateAsync(Application updatedApplication, Guid userId)
        {
            var application = await _findApplicationService.FindAsync(updatedApplication.ApplicationId, userId);

            // If the application is going public, we don't want a secret laying around.
            if (updatedApplication.Type == ClientTypes.Confidential && updatedApplication.Type == ClientTypes.Public)
            {
                application.ClientSecret = null;
            }

            application.Update(updatedApplication);
            await _applicationRepository.SaveAsync();

            return application;
        }
    }
}
