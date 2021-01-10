using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AddScopeService : IAddScopeService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IScopeRepository       _scopeRepository;

        public AddScopeService(IScopeRepository scopeRepository, IApplicationRepository applicationRepository)
        {
            _scopeRepository       = scopeRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<Scope> AddAsync(Scope scope)
        {
            Application application = await _applicationRepository.FindAsync(scope.ApplicationId);
            if (application == null) { throw new BadRequestException("Application does not exist."); }

            return await AddAsync(scope, application);
        }

        public async Task<Scope> AddAsync(Scope scope, Guid userId)
        {
            Application application = await _applicationRepository.FindAsync(scope.ApplicationId);
            if (application?.UserId != userId) { throw new ForbiddenException(); }

            return await AddAsync(scope, application);
        }

        private async Task<Scope> AddAsync(Scope scope, Application application)
        {
            if (application.Scopes.Any(s => s.Name == scope.Name)) { throw new ConflictException("Scope name already exists."); }

            _scopeRepository.Add(scope);
            await _scopeRepository.SaveAsync();

            return scope;
        }
    }
}
