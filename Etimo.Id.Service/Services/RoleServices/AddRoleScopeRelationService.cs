using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AddRoleScopeRelationService : IAddRoleScopeRelationService
    {
        private readonly IFindRoleService _findRoleService;
        private readonly IRoleRepository _roleRepository;
        private readonly IScopeRepository _scopeRepository;

        public AddRoleScopeRelationService(
            IFindRoleService findRoleService,
            IRoleRepository roleRepository,
            IScopeRepository scopeRepository)
        {
            _findRoleService = findRoleService;
            _roleRepository = roleRepository;
            _scopeRepository = scopeRepository;
        }

        public async Task<List<Scope>> AddScopeRelationAsync(Guid roleId, Guid scopeId)
        {
            var role = await _findRoleService.FindAsync(roleId);

            return await AddScopeRelationAsync(role, scopeId);
        }

        public async Task<List<Scope>> AddScopeRelationAsync(Guid roleId, Guid scopeId, Guid userId)
        {
            var role = await _findRoleService.FindAsync(roleId);
            if (role.Application.UserId != userId)
            {
                throw new ForbiddenException();
            }

            return await AddScopeRelationAsync(role, scopeId);
        }

        private async Task<List<Scope>> AddScopeRelationAsync(Role role, Guid scopeId)
        {
            if (!role.Scopes.Any(s => s.ScopeId == scopeId))
            {
                var scope = await _scopeRepository.FindAsync(scopeId);
                if (scope == null)
                {
                    throw new NotFoundException();
                }

                role.Scopes.Add(scope);
                await _roleRepository.SaveAsync();
            }

            return role.Scopes.ToList();
        }
    }
}
