using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class GetRolesService : IGetRolesService
    {
        private readonly IFindApplicationService _findApplicationService;
        private readonly IRoleRepository _roleRepository;

        public GetRolesService(
            IFindApplicationService findApplicationService,
            IRoleRepository roleRepository)
        {
            _findApplicationService = findApplicationService;
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

        public Task<List<Role>> GetByUserIdAsync(Guid userId)
        {
            return _roleRepository.GetByUserIdAsync(userId);
        }
    }
}
