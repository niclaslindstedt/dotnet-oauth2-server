using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class FindScopeService : IFindScopeService
    {
        private readonly IScopeRepository _scopeRepository;

        public FindScopeService(IScopeRepository scopeRepository)
        {
            _scopeRepository = scopeRepository;
        }

        public async Task<Scope> FindAsync(Guid scopeId)
        {
            Scope scope = await _scopeRepository.FindAsync(scopeId);
            if (scope == null) { throw new NotFoundException(); }

            return scope;
        }

        public async Task<Scope> FindAsync(Guid scopeId, Guid userId)
        {
            Scope scope = await _scopeRepository.FindAsync(scopeId);
            if (scope.Application.UserId != userId) { throw new NotFoundException(); }

            return scope;
        }
    }
}
