using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class GetRolesService : IGetRolesService
    {
        private readonly IFindApplicationService _findApplicationService;
        private readonly IFindScopeService       _findScopeService;
        private readonly IRoleRepository         _roleRepository;

        public GetRolesService(
            IFindApplicationService findApplicationService,
            IFindScopeService findScopeService,
            IRoleRepository roleRepository)
        {
            _findApplicationService = findApplicationService;
            _findScopeService       = findScopeService;
            _roleRepository         = roleRepository;
        }

        public Task<List<Role>> GetAllAsync()
            => _roleRepository.GetAllAsync();

        public Task<List<Role>> GetByApplicationId(int applicationId)
            => _roleRepository.GetByApplicationId(applicationId);

        public async Task<List<Role>> GetByApplicationId(int applicationId, Guid userId)
        {
            await _findApplicationService.FindAsync(applicationId, userId);

            return await GetByApplicationId(applicationId);
        }

        public async Task<List<Role>> GetByScopeIdAsync(Guid scopeId)
        {
            List<Role> roles = await _roleRepository.GetByScopeIdAsync(scopeId);

            return roles.ToList();
        }

        public async Task<List<Role>> GetByScopeIdAsync(Guid scopeId, Guid userId)
        {
            Scope scope = await _findScopeService.FindAsync(scopeId, userId);

            return scope.Roles?.ToList();
        }

        public Task<List<Role>> GetByUserIdAsync(Guid userId)
            => _roleRepository.GetByUserIdAsync(userId);
    }
}
