using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class GetScopesService : IGetScopesService
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly IApplicationRepository _applicationRepository;

        public GetScopesService(
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
    }
}
