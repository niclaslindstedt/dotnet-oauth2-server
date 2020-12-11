using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class UpdateScopeService : IUpdateScopeService
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly IApplicationRepository _applicationRepository;

        public UpdateScopeService(
            IScopeRepository scopeRepository,
            IApplicationRepository applicationRepository)
        {
            _scopeRepository = scopeRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<Scope> UpdateAsync(Scope updatedScope)
        {
            var scope = await _scopeRepository.FindAsync(updatedScope.ScopeId);
            if (scope == null)
            {
                throw new BadRequestException("Scope does not exist.");
            }

            return await UpdateAsync(scope, updatedScope);
        }

        public async Task<Scope> UpdateAsync(Scope updatedScope, Guid userId)
        {
            var scope = await _scopeRepository.FindAsync(updatedScope.ScopeId);
            if (scope?.Application?.UserId != userId)
            {
                throw new ForbiddenException();
            }

            return await UpdateAsync(scope, updatedScope);
        }

        private async Task<Scope> UpdateAsync(Scope scope, Scope updatedScope)
        {
            if (updatedScope.ApplicationId != scope.ApplicationId)
            {
                throw new BadRequestException("Cannot change application of a scope.");
            }

            scope.Update(updatedScope);
            await _scopeRepository.SaveAsync();

            return scope;
        }
    }
}
