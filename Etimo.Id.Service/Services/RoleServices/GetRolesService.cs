using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class GetRolesService : IGetRolesService
    {
        private readonly IRoleRepository _roleRepository;

        public GetRolesService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public Task<List<Role>> GetAllAsync()
        {
            return _roleRepository.GetAllAsync();
        }

        public Task<List<Role>> GetByUserIdAsync(Guid userId)
        {
            return _roleRepository.GetByUserIdAsync(userId);
        }
    }
}
