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
        private readonly IScopeRepository _scopeRepository;
        private readonly IApplicationRepository _applicationRepository;

        public ScopeService(
            IScopeRepository scopeRepository,
            IApplicationRepository applicationRepository)
        {
            _scopeRepository = scopeRepository;
            _applicationRepository = applicationRepository;
        }

        public Task<List<Scope>> GetAllAsync()
        {
            return _scopeRepository.GetAllAsync();
        }

        public async Task<List<Scope>> GetByClientIdAsync(Guid clientId)
        {
            var applications = await _applicationRepository.GetByUserIdAsync(clientId);

            return applications.SelectMany(a => a.Scopes).ToList();
        }

        public async Task<Scope> FindAsync(Guid scopeId)
        {
            var scope = await _scopeRepository.FindAsync(scopeId);
            if (scope == null)
            {
                throw new NotFoundException();
            }

            return scope;
        }

        public async Task<Scope> FindAsync(Guid scopeId, Guid userId)
        {
            var scope = await _scopeRepository.FindAsync(scopeId);
            if (scope.Application.UserId != userId)
            {
                throw new NotFoundException();
            }

            return scope;
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

        public async Task<Scope> UpdateAsync(Scope updatedScope)
        {
            var scope = await _scopeRepository.FindAsync(updatedScope.ScopeId);

            return await UpdateAsync(scope, updatedScope);
        }

        public async Task<Scope> UpdateAsync(Scope updatedScope, Guid userId)
        {
            var scope = await _scopeRepository.FindAsync(updatedScope.ScopeId);
            if (scope?.Application?.UserId != userId)
            {
                throw new ForbiddenException("The application does not belong to you.");
            }

            return await UpdateAsync(scope, updatedScope);
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

        public async Task<Scope> UpdateAsync(Scope scope, Scope updatedScope)
        {
            if (scope == null)
            {
                throw new BadRequestException("Scope does not exist.");
            }

            if (updatedScope.ApplicationId != scope.ApplicationId)
            {
                throw new BadRequestException("Cannot change application of a scope.");
            }

            scope.Update(updatedScope);
            await _scopeRepository.SaveAsync();

            return scope;
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
