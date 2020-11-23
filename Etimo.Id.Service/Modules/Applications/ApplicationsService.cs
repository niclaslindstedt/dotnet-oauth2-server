using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service.Applications
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

        public Task<List<Application>> GetByUserIdAsync(Guid userId)
        {
            return _applicationsRepository.GetByUserIdAsync(userId);
        }

        public Task<List<Application>> GetAllAsync()
        {
            return _applicationsRepository.GetAllAsync();
        }

        public async Task<Application> AddAsync(Application application, Guid userId)
        {
            application.UserId = userId;
            _applicationsRepository.Add(application);
            await _applicationsRepository.SaveAsync();

            return application;
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

        public async Task DeleteAsync(int applicationId)
        {
            var client = await _applicationsRepository.FindAsync(applicationId);
            await DeleteAsync(client);
        }

        public async Task DeleteAsync(int applicationId, Guid userId)
        {
            var client = await _applicationsRepository.FindAsync(applicationId);
            if (client.UserId == userId)
            {
                await DeleteAsync(client);
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
            var client = await _applicationsRepository.FindAsync(clientId);
            if (client == null)
            {
                throw new InvalidGrantException();
            }

            if (!_passwordHasher.Verify(clientSecret, client.ClientSecret))
            {
                throw new InvalidGrantException();
            }

            return client;
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
