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
        private readonly IScopesRepository _scopesRepository;
        private readonly IApplicationsRepository _applicationsRepository;

        public ScopeService(
            IScopesRepository scopesRepository,
            IApplicationsRepository applicationsRepository)
        {
            _scopesRepository = scopesRepository;
            _applicationsRepository = applicationsRepository;
        }

        public async Task<List<Scope>> GetByClientIdAsync(Guid clientId)
        {
            var applications = await _applicationsRepository.GetByUserIdAsync(clientId);

            return applications.SelectMany(a => a.Scopes).ToList();
        }

        public Task<List<Scope>> GetAllAsync()
        {
            return _scopesRepository.GetAllAsync();
        }

        public async Task<Scope> FindAsync(Guid scopeId)
        {
            var scope = await _scopesRepository.FindAsync(scopeId);
            if (scope == null)
            {
                throw new NotFoundException("Scope does not exist or does not belong to any applications you own.");
            }

            return scope;
        }

        public async Task<Scope> FindAsync(Guid scopeId, Guid userId)
        {
            var scope = await _scopesRepository.FindAsync(scopeId);
            if (scope.Application.UserId != userId)
            {
                throw new NotFoundException("Scope does not exist or does not belong to any applications you own.");
            }

            return scope;
        }

        public async Task<Scope> AddAsync(Scope scope, Guid userId)
        {
            var application = await _applicationsRepository.FindAsync(scope.ApplicationId);
            if (application.UserId != userId)
            {
                throw new BadRequestException("Application does not exist or does not belong to you.");
            }

            if (application.Scopes.Any(s => s.Name == scope.Name))
            {
                throw new BadRequestException("Scope name is used by another Scope.");
            }

            _scopesRepository.Add(scope);
            await _scopesRepository.SaveAsync();

            return scope;
        }

        public async Task<Scope> UpdateAsync(Scope updatedScope, Guid userId)
        {
            var scope = await _scopesRepository.FindAsync(updatedScope.ScopeId);
            if (scope.Application.UserId != userId)
            {
                throw new BadRequestException("Application does not exist or does not belong to you.");
            }

            if (updatedScope.ApplicationId != scope.ApplicationId)
            {
                throw new BadRequestException("A scope cannot change application owner.");
            }

            scope.Update(updatedScope);
            await _scopesRepository.SaveAsync();

            return scope;
        }

        public async Task DeleteAsync(Guid scopeId)
        {
            var scope = await _scopesRepository.FindAsync(scopeId);
            await DeleteAsync(scope);
        }

        public async Task DeleteAsync(Guid scopeId, Guid userId)
        {
            var scope = await _scopesRepository.FindAsync(scopeId);
            if (scope?.Application.UserId == userId)
            {
                await DeleteAsync(scope);
            }
        }

        private async Task DeleteAsync(Scope scope)
        {
            if (scope != null)
            {
                _scopesRepository.Delete(scope);
                await _scopesRepository.SaveAsync();
            }
        }
    }
}
