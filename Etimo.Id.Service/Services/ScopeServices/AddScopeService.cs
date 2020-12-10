using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AddScopeService : IAddScopeService
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly IApplicationRepository _applicationRepository;

        public AddScopeService(
            IScopeRepository scopeRepository,
            IApplicationRepository applicationRepository)
        {
            _scopeRepository = scopeRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<Scope> AddAsync(Scope scope)
        {
            var application = await _applicationRepository.FindAsync(scope.ApplicationId);

            return await AddAsync(scope, application);
        }

        public async Task<Scope> AddAsync(Scope scope, Guid userId)
        {
            var application = await _applicationRepository.FindAsync(scope.ApplicationId);
            if (application?.UserId != userId)
            {
                throw new ForbiddenException("The application does not belong to you.");
            }

            return await AddAsync(scope, application);
        }

        private async Task<Scope> AddAsync(Scope scope, Application application)
        {
            if (application == null)
            {
                throw new BadRequestException("Application does not exist.");
            }

            if (application.Scopes.Any(s => s.Name == scope.Name))
            {
                throw new ConflictException("Scope name already exists.");
            }

            _scopeRepository.Add(scope);
            await _scopeRepository.SaveAsync();

            return scope;
        }
    }
}
