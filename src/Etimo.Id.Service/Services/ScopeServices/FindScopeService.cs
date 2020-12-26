using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
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
    }
}
