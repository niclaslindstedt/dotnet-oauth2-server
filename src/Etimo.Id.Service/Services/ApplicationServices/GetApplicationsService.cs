using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class GetApplicationsService : IGetApplicationsService
    {
        private readonly IApplicationRepository _applicationRepository;

        public GetApplicationsService(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public Task<List<Application>> GetAllAsync()
        {
            return _applicationRepository.GetAllAsync();
        }

        public Task<List<Application>> GetByUserIdAsync(Guid userId)
        {
            return _applicationRepository.GetByUserIdAsync(userId);
        }
    }
}
