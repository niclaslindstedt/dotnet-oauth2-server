using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class DeleteRoleService : IDeleteRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public DeleteRoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task DeleteAsync(Guid roleId)
        {
            Role role = await _roleRepository.FindAsync(roleId);
            if (role != null) { await DeleteAsync(role); }
        }

        public async Task DeleteAsync(Guid roleId, Guid userId)
        {
            Role role = await _roleRepository.FindAsync(roleId);
            if (role?.Application?.UserId == userId) { await DeleteAsync(role); }
        }

        private async Task DeleteAsync(Role role)
        {
            _roleRepository.Delete(role);
            await _roleRepository.SaveAsync();
        }
    }
}
