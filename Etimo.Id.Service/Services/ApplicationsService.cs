using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class ApplicationsService : IApplicationsService
    {
        private readonly IApplicationsRepository _applicationsRepository;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IPasswordHasher _passwordHasher;

        public ApplicationsService(
            IApplicationsRepository applicationsRepository,
            IPasswordGenerator passwordGenerator,
            IPasswordHasher passwordHasher)
        {
            _applicationsRepository = applicationsRepository;
            _passwordGenerator = passwordGenerator;
            _passwordHasher = passwordHasher;
        }

        public Task<List<Application>> GetAllAsync()
        {
            return _applicationsRepository.GetAllAsync();
        }

        public Task<List<Application>> GetByUserIdAsync(Guid userId)
        {
            return _applicationsRepository.GetByUserIdAsync(userId);
        }

        public async ValueTask<Application> FindAsync(int applicationId)
        {
            var app = await _applicationsRepository.FindAsync(applicationId);

            if (app == null)
            {
                throw new NotFoundException();
            }

            return app;
        }

        public async Task<Application> FindAsync(int applicationId, Guid userId)
        {
            var app = await _applicationsRepository.FindAsync(applicationId);

            if (app?.UserId != userId)
            {
                throw new NotFoundException();
            }

            return app;
        }

        public Task<Application> FindByClientIdAsync(Guid clientId)
        {
            return _applicationsRepository.FindByClientIdAsync(clientId);
        }

        public async Task<Application> AddAsync(Application application, Guid userId)
        {
            application.UserId = userId;
            _applicationsRepository.Add(application);
            await _applicationsRepository.SaveAsync();

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

            application.MergeWith(updatedApp);
            await _applicationsRepository.SaveAsync();

            return application;
        }

        public async Task DeleteAsync(int applicationId)
        {
            var application = await _applicationsRepository.FindAsync(applicationId);
            await DeleteAsync(application);
        }

        public async Task DeleteAsync(int applicationId, Guid userId)
        {
            var application = await _applicationsRepository.FindAsync(applicationId);
            if (application?.UserId == userId)
            {
                await DeleteAsync(application);
            }
        }

        private async Task DeleteAsync(Application application)
        {
            if (application != null)
            {
                _applicationsRepository.Delete(application);
                await _applicationsRepository.SaveAsync();
            }
        }

        public async Task<Application> AuthenticateAsync(Guid clientId, string clientSecret)
        {
            var application = await _applicationsRepository.FindAsync(clientId);
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
            await _applicationsRepository.SaveAsync();

            application.ClientSecret = secret;

            return application;
        }
    }
}
