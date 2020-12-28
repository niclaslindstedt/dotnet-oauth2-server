using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AuthenticateClientService : IAuthenticateClientService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IPasswordHasher        _passwordHasher;

        public AuthenticateClientService(IApplicationRepository applicationRepository, IPasswordHasher passwordHasher)
        {
            _applicationRepository = applicationRepository;
            _passwordHasher        = passwordHasher;
        }

        public async Task<Application> AuthenticateAsync(Guid clientId, string clientSecret)
        {
            Application application = await _applicationRepository.FindAsync(clientId);
            if (application == null) { throw new InvalidGrantException("Invalid client id."); }

            if (!_passwordHasher.Verify(clientSecret, application.ClientSecret))
            {
                throw new InvalidGrantException("Invalid client credentials.");
            }

            return application;
        }
    }
}
