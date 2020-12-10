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
        private readonly IFindScopeService _findScopeService;
        private readonly IRoleRepository _roleRepository;

        public GetRolesService(
            IFindApplicationService findApplicationService,
            IFindScopeService findScopeService,
            IRoleRepository roleRepository)
        {
            _findApplicationService = findApplicationService;
            _findScopeService = findScopeService;
            _roleRepository = roleRepository;
        }

        public Task<List<Role>> GetAllAsync()
        {
            return _roleRepository.GetAllAsync();
        }

        public Task<List<Role>> GetByApplicationId(int applicationId)
        {
            return _roleRepository.GetByApplicationId(applicationId);
        }

        public async Task<List<Role>> GetByApplicationId(int applicationId, Guid userId)
        {
            await _findApplicationService.FindAsync(applicationId, userId);

            return await GetByApplicationId(applicationId);
        }

        public async Task<List<Role>> GetByScopeIdAsync(Guid scopeId)
        {
            var roles = await _roleRepository.GetByScopeIdAsync(scopeId);

            return roles.ToList();
        }

        public async Task<List<Role>> GetByScopeIdAsync(Guid scopeId, Guid userId)
        {
            var scope = await _findScopeService.FindAsync(scopeId, userId);

            return scope.Roles?.ToList();
        }

        public Task<List<Role>> GetByUserIdAsync(Guid userId)
        {
            return _roleRepository.GetByUserIdAsync(userId);
        }
    }
}
