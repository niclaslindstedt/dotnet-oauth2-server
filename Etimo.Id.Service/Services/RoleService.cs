using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IScopeRepository _scopeRepository;

        public RoleService(
            IRoleRepository roleRepository,
            IApplicationRepository applicationRepository,
            IScopeRepository scopeRepository)
        {
            _roleRepository = roleRepository;
            _applicationRepository = applicationRepository;
            _scopeRepository = scopeRepository;
        }

        public Task<List<Role>> GetAllAsync()
        {
            return _roleRepository.GetAllAsync();
        }

        public Task<List<Role>> GetByUserIdAsync(Guid userId)
        {
            return _roleRepository.GetByUserIdAsync(userId);
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

        public async Task<Role> AddAsync(Role role)
        {
            var application = await _applicationRepository.FindAsync(role.ApplicationId);

            return await AddAsync(role, application);
        }

        public async Task<Role> AddAsync(Role role, Guid userId)
        {
            var application = await _applicationRepository.FindAsync(role.ApplicationId);
            if (application?.UserId != userId)
            {
                throw new ForbiddenException("Application does not belong to you.");
            }

            return await AddAsync(role, application);
        }

        public async Task<Role> UpdateAsync(Role updatedRole)
        {
            var role = await FindAsync(updatedRole.RoleId);

            return await UpdateAsync(role, updatedRole);
        }

        public async Task<Role> UpdateAsync(Role updatedRole, Guid userId)
        {
            var role = await FindAsync(updatedRole.RoleId, userId);

            return await UpdateAsync(role, updatedRole);
        }

        public async Task DeleteAsync(Guid roleId)
        {
            var role = await _roleRepository.FindAsync(roleId);
            if (role != null)
            {
                await DeleteAsync(role);
            }
        }

        public async Task DeleteAsync(Guid roleId, Guid userId)
        {
            var role = await _roleRepository.FindAsync(roleId);
            if (role?.Application?.UserId == userId)
            {
                await DeleteAsync(role);
            }
        }

        public async Task<Role> AddScopeRelationAsync(Guid roleId, Guid scopeId)
        {
            var role = await FindAsync(roleId);

            return await AddScopeRelationAsync(role, scopeId);
        }

        public async Task<Role> AddScopeRelationAsync(Guid roleId, Guid scopeId, Guid userId)
        {
            var role = await FindAsync(roleId);
            if (role.Application.UserId != userId)
            {
                throw new ForbiddenException("Role does not belong to you.");
            }

            return await AddScopeRelationAsync(role, scopeId);
        }

        public async Task<Role> DeleteScopeRelationAsync(Guid roleId, Guid scopeId)
        {
            var role = await FindAsync(roleId);

            return await DeleteScopeRelationAsync(role, scopeId);
        }

        public async Task<Role> DeleteScopeRelationAsync(Guid roleId, Guid scopeId, Guid userId)
        {
            var role = await FindAsync(roleId);
            if (role.Application.UserId != userId)
            {
                throw new ForbiddenException("Role does not belong to you.");
            }

            return await DeleteScopeRelationAsync(role, scopeId);
        }

        private async Task<Role> AddAsync(Role role, Application application)
        {
            if (application == null)
            {
                throw new BadRequestException("The application does not exist.");
            }

            if (application.Roles.Any(r => r.Name == role.Name))
            {
                throw new ConflictException("A role with that name already exists in this application.");
            }

            _roleRepository.Add(role);
            await _roleRepository.SaveAsync();

            return role;
        }

        private async Task<Role> UpdateAsync(Role role, Role updatedRole)
        {
            role.Update(updatedRole);
            await _roleRepository.SaveAsync();

            return role;
        }

        private async Task DeleteAsync(Role role)
        {
            _roleRepository.Delete(role);
            await _roleRepository.SaveAsync();
        }

        private async Task<Role> AddScopeRelationAsync(Role role, Guid scopeId)
        {
            if (!role.Scopes.Any(s => s.ScopeId == scopeId))
            {
                var scope = await _scopeRepository.FindAsync(scopeId);
                if (scope == null)
                {
                    throw new BadRequestException("Scope not found.");
                }

                role.Scopes.Add(scope);
                await _roleRepository.SaveAsync();
            }

            return role;
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
