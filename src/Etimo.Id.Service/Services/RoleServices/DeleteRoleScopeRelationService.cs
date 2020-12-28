using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class DeleteRoleScopeRelationService : IDeleteRoleScopeRelationService
    {
        private readonly IFindRoleService _findRoleService;
        private readonly IRoleRepository  _roleRepository;
        private readonly IScopeRepository _scopeRepository;

        public DeleteRoleScopeRelationService(
            IFindRoleService findRoleService,
            IRoleRepository roleRepository,
            IScopeRepository scopeRepository)
        {
            _findRoleService = findRoleService;
            _roleRepository  = roleRepository;
            _scopeRepository = scopeRepository;
        }

        public async Task<List<Scope>> DeleteScopeRelationAsync(Guid roleId, Guid scopeId)
        {
            Role role = await _findRoleService.FindAsync(roleId);

            return await DeleteScopeRelationAsync(role, scopeId);
        }

        public async Task<List<Scope>> DeleteScopeRelationAsync(
            Guid roleId,
            Guid scopeId,
            Guid userId)
        {
            Role role = await _findRoleService.FindAsync(roleId);
            if (role.Application.UserId != userId) { throw new ForbiddenException(); }

            return await DeleteScopeRelationAsync(role, scopeId);
        }

        private async Task<List<Scope>> DeleteScopeRelationAsync(Role role, Guid scopeId)
        {
            if (role.Scopes.Any(s => s.ScopeId == scopeId))
            {
                Scope scope = await _scopeRepository.FindAsync(scopeId);
                if (scope == null) { throw new BadRequestException("Scope not found."); }

                role.Scopes.Remove(scope);
                await _roleRepository.SaveAsync();
            }

            return role.Scopes.ToList();
        }
    }
}
