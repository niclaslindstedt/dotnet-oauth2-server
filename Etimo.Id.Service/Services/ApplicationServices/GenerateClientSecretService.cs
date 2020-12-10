using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class GenerateClientSecretService : IGenerateClientSecretService
    {
        private readonly IFindApplicationService _findApplicationService;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IPasswordHasher _passwordHasher;

        public GenerateClientSecretService(
            IFindApplicationService findApplicationService,
            IApplicationRepository applicationRepository,
            IPasswordGenerator passwordGenerator,
            IPasswordHasher passwordHasher)
        {
            _findApplicationService = findApplicationService;
            _applicationRepository = applicationRepository;
            _passwordGenerator = passwordGenerator;
            _passwordHasher = passwordHasher;
        }

        public async Task<Application> GenerateSecretAsync(int applicationId)
        {
            var application = await _findApplicationService.FindAsync(applicationId);

            return await GenerateSecretAsync(application);
        }

        public async Task<Application> GenerateSecretAsync(int applicationId, Guid userId)
        {
            var application = await _findApplicationService.FindAsync(applicationId, userId);

            return await GenerateSecretAsync(application);
        }

        private async Task<Application> GenerateSecretAsync(Application application)
        {
            var secret = _passwordGenerator.Generate(64);
            application.ClientSecret = _passwordHasher.Hash(secret);
            await _applicationRepository.SaveAsync();

            application.ClientSecret = secret;

            return application;
        }
    }
}
