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
        private readonly IApplicationRepository _applicationRepository;
        private readonly IFindRoleService       _findRoleService;
        private readonly IScopeRepository       _scopeRepository;

        public GetScopesService(
            IFindRoleService findRoleService,
            IScopeRepository scopeRepository,
            IApplicationRepository applicationRepository)
        {
            _findRoleService       = findRoleService;
            _scopeRepository       = scopeRepository;
            _applicationRepository = applicationRepository;
        }

        public Task<List<Scope>> GetAllAsync()
            => _scopeRepository.GetAllAsync();

        public async Task<List<Scope>> GetByClientIdAsync(Guid clientId)
        {
            List<Application> applications = await _applicationRepository.GetByUserIdAsync(clientId);

            return applications.SelectMany(a => a.Scopes).ToList();
        }

        public async Task<List<Scope>> GetByRoleIdAsync(Guid roleId)
        {
            Role role = await _findRoleService.FindAsync(roleId);

            return role.Scopes.ToList();
        }

        public async Task<List<Scope>> GetByRoleIdAsync(Guid roleId, Guid userId)
        {
            Role role = await _findRoleService.FindAsync(roleId, userId);

            return role.Scopes.ToList();
        }
    }
}
