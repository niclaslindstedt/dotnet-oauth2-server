using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AddApplicationService : IAddApplicationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IApplicationRepository _applicationRepository;

        public AddApplicationService(
            IUserRepository userRepository,
            IApplicationRepository applicationRepository)
        {
            _userRepository = userRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<Application> AddAsync(Application application, Guid userId)
        {
            if (!await _userRepository.AnyAsync(userId))
            {
                throw new BadRequestException("User does not exist.");
            }

            application.UserId = userId;

            _applicationRepository.Add(application);
            await _applicationRepository.SaveAsync();

            return application;
        }
    }
}
