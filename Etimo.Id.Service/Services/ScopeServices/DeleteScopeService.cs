using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class DeleteScopeService : IDeleteScopeService
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly IApplicationRepository _applicationRepository;

        public DeleteScopeService(
            IScopeRepository scopeRepository,
            IApplicationRepository applicationRepository)
        {
            _scopeRepository = scopeRepository;
            _applicationRepository = applicationRepository;
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
