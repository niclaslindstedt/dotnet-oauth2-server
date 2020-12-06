using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class ScopeService : IScopeService
    {
        private readonly IScopesRepository _scopeRepository;
        private readonly IApplicationRepository _applicationRepository;

        public ScopeService(
            IScopesRepository scopeRepository,
            IApplicationRepository applicationRepository)
        {
            _scopeRepository = scopeRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<List<Scope>> GetByClientIdAsync(Guid clientId)
        {
            var applications = await _applicationRepository.GetByUserIdAsync(clientId);

            return applications.SelectMany(a => a.Scopes).ToList();
        }

        public Task<List<Scope>> GetAllAsync()
        {
            return _scopeRepository.GetAllAsync();
        }

        public async Task<Scope> FindAsync(Guid scopeId)
        {
            var scope = await _scopeRepository.FindAsync(scopeId);
            if (scope == null)
            {
                throw new NotFoundException("Scope does not exist or does not belong to any applications you own.");
            }

            return scope;
        }

        public async Task<Scope> FindAsync(Guid scopeId, Guid userId)
        {
            var scope = await _scopeRepository.FindAsync(scopeId);
            if (scope.Application.UserId != userId)
            {
                throw new NotFoundException("Scope does not exist or does not belong to any applications you own.");
            }

            return scope;
        }

        public async Task<Scope> AddAsync(Scope scope, Guid userId)
        {
            var application = await _applicationRepository.FindAsync(scope.ApplicationId);
            if (application.UserId != userId)
            {
                throw new BadRequestException("Application does not exist or does not belong to you.");
            }

            if (application.Scopes.Any(s => s.Name == scope.Name))
            {
                throw new BadRequestException("Scope name is used by another Scope.");
            }

            _scopeRepository.Add(scope);
            await _scopeRepository.SaveAsync();

            return scope;
        }

        public async Task<Scope> UpdateAsync(Scope updatedScope, Guid userId)
        {
            var scope = await _scopeRepository.FindAsync(updatedScope.ScopeId);
            if (scope.Application.UserId != userId)
            {
                throw new BadRequestException("Application does not exist or does not belong to you.");
            }

            if (updatedScope.ApplicationId != scope.ApplicationId)
            {
                throw new BadRequestException("A scope cannot change application owner.");
            }

            scope.Update(updatedScope);
            await _scopeRepository.SaveAsync();

            return scope;
        }

        public async Task DeleteAsync(Guid scopeId)
        {
            var scope = await _scopeRepository.FindAsync(scopeId);
            await DeleteAsync(scope);
        }

        public async Task DeleteAsync(Guid scopeId, Guid userId)
        {
            var scope = await _scopeRepository.FindAsync(scopeId);
            if (scope?.Application.UserId == userId)
            {
                await DeleteAsync(scope);
            }
        }

        private async Task DeleteAsync(Scope scope)
        {
            if (scope != null)
            {
                _scopeRepository.Delete(scope);
                await _scopeRepository.SaveAsync();
            }
        }
    }
}
