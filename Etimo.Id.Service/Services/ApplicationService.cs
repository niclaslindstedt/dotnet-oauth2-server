using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IPasswordHasher _passwordHasher;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IPasswordGenerator passwordGenerator,
            IPasswordHasher passwordHasher)
        {
            _applicationRepository = applicationRepository;
            _passwordGenerator = passwordGenerator;
            _passwordHasher = passwordHasher;
        }

        public Task<List<Application>> GetAllAsync()
        {
            return _applicationRepository.GetAllAsync();
        }

        public Task<List<Application>> GetByUserIdAsync(Guid userId)
        {
            return _applicationRepository.GetByUserIdAsync(userId);
        }

        public async ValueTask<Application> FindAsync(int applicationId)
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

        public async Task<Application> AddAsync(Application application, Guid userId)
        {
            application.UserId = userId;
            _applicationRepository.Add(application);
            await _applicationRepository.SaveAsync();

            return application;
        }

        public async Task<Application> UpdateAsync(Application updatedApp, Guid userId)
        {
            var application = await FindAsync(updatedApp.ApplicationId, userId);

            // If the application is going public, we don't want a secret laying around.
            if (updatedApp.Type == ClientTypes.Confidential && updatedApp.Type == ClientTypes.Public)
            {
                application.ClientSecret = null;
            }

            application.Update(updatedApp);
            await _applicationRepository.SaveAsync();

            return application;
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

        public async Task<Application> AuthenticateAsync(Guid clientId, string clientSecret)
        {
            var application = await _applicationRepository.FindAsync(clientId);
            if (application == null)
            {
                throw new InvalidGrantException("Invalid client id.");
            }

            if (!_passwordHasher.Verify(clientSecret, application.ClientSecret))
            {
                throw new InvalidGrantException("Invalid client credentials.");
            }

            return application;
        }

        public async Task<Application> GenerateSecretAsync(int applicationId, Guid userId)
        {
            var application = await FindAsync(applicationId, userId);

            var secret = _passwordGenerator.Generate(64);
            application.ClientSecret = _passwordHasher.Hash(secret);
            await _applicationRepository.SaveAsync();

            application.ClientSecret = secret;

            return application;
        }
    }
}
