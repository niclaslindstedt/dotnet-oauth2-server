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

        public RoleService(
            IRoleRepository roleRepository,
            IApplicationRepository applicationRepository)
        {
            _roleRepository = roleRepository;
            _applicationRepository = applicationRepository;
        }

        public Task<List<Role>> GetAllAsync()
        {
            return _roleRepository.GetAllAsync();
        }

        public async Task<Role> FindAsync(Guid roleId)
        {
            var role = await _roleRepository.FindAsync(roleId);
            if (role == null)
            {
                throw new NotFoundException("Role not found.");
            }

            return role;
        }

        public async Task<Role> FindAsync(Guid roleId, Guid userId)
        {
            var role = await FindAsync(roleId);
            if (role.Application.UserId != userId)
            {
                throw new NotFoundException("Role not found.");
            }

            return role;
        }

        public async Task<Role> AddAsync(Role role, Guid userId)
        {
            var application = await _applicationRepository.FindAsync(role.ApplicationId);
            if (application == null || application.UserId != userId)
            {
                throw new BadRequestException("Application not found or does not belong to you.");
            }

            if (application.Roles.Any(r => r.Name == role.Name))
            {
                throw new BadRequestException("A role with that name already exists in this application.");
            }

            _roleRepository.Add(role);
            await _roleRepository.SaveAsync();

            return role;
        }

        public async Task<Role> UpdateAsync(Role updatedRole, Guid userId)
        {
            var role = await FindAsync(updatedRole.RoleId, userId);

            role.Update(updatedRole);
            await _roleRepository.SaveAsync();

            return role;
        }

        public async Task DeleteAsync(Guid roleId)
        {
            var role = await _roleRepository.FindAsync(roleId);
            if (role != null)
            {
                _roleRepository.Delete(role);
                await _roleRepository.SaveAsync();
            }
        }
    }
}
