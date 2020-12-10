using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class DeleteRoleScopeRelationService : IDeleteRoleScopeRelationService
    {
        private readonly IFindRoleService _findRoleService;
        private readonly IRoleRepository _roleRepository;
        private readonly IScopeRepository _scopeRepository;

        public DeleteRoleScopeRelationService(
            IFindRoleService findRoleService,
            IRoleRepository roleRepository,
            IScopeRepository scopeRepository)
        {
            _findRoleService = findRoleService;
            _roleRepository = roleRepository;
            _scopeRepository = scopeRepository;
        }

        public async Task<Role> DeleteScopeRelationAsync(Guid roleId, Guid scopeId)
        {
            var role = await _findRoleService.FindAsync(roleId);

            return await DeleteScopeRelationAsync(role, scopeId);
        }

        public async Task<Role> DeleteScopeRelationAsync(Guid roleId, Guid scopeId, Guid userId)
        {
            var role = await _findRoleService.FindAsync(roleId);
            if (role.Application.UserId != userId)
            {
                throw new ForbiddenException("Role does not belong to you.");
            }

            return await DeleteScopeRelationAsync(role, scopeId);
        }

        private async Task<Role> DeleteScopeRelationAsync(Role role, Guid scopeId)
        {
            if (role.Scopes.Any(s => s.ScopeId == scopeId))
            {
                var scope = await _scopeRepository.FindAsync(scopeId);
                if (scope == null)
                {
                    throw new BadRequestException("Scope not found.");
                }

                role.Scopes.Remove(scope);
                await _roleRepository.SaveAsync();
            }

            return role;
        }
    }
}