using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class UpdateRoleService : IUpdateRoleService
    {
        private readonly IFindRoleService _findRoleService;
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleService(
            IFindRoleService findRoleService,
            IRoleRepository roleRepository)
        {
            _findRoleService = findRoleService;
            _roleRepository = roleRepository;
        }

        public async Task<Role> UpdateAsync(Role updatedRole)
        {
            var role = await _findRoleService.FindAsync(updatedRole.RoleId);

            return await UpdateAsync(role, updatedRole);
        }

        public async Task<Role> UpdateAsync(Role updatedRole, Guid userId)
        {
            var role = await _findRoleService.FindAsync(updatedRole.RoleId, userId);

            return await UpdateAsync(role, updatedRole);
        }

        private async Task<Role> UpdateAsync(Role role, Role updatedRole)
        {
            role.Update(updatedRole);
            await _roleRepository.SaveAsync();

            return role;
        }
    }
}
