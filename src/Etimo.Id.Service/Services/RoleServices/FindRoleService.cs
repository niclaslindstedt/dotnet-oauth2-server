using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class FindRoleService : IFindRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public FindRoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Role> FindAsync(Guid roleId)
        {
            var role = await _roleRepository.FindAsync(roleId);
            if (role == null)
            {
                throw new NotFoundException();
            }

            return role;
        }

        public async Task<Role> FindAsync(Guid roleId, Guid userId)
        {
            var role = await FindAsync(roleId);
            if (role.Application.UserId != userId)
            {
                throw new NotFoundException();
            }

            return role;
        }
    }
}
