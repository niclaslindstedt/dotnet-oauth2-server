using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AddRoleService : IAddRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IApplicationRepository _applicationRepository;

        public AddRoleService(
            IRoleRepository roleRepository,
            IApplicationRepository applicationRepository)
        {
            _roleRepository = roleRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<Role> AddAsync(Role role)
        {
            var application = await _applicationRepository.FindAsync(role.ApplicationId);
            if (application == null)
            {
                throw new BadRequestException("The application does not exist.");
            }

            return await AddAsync(role, application);
        }

        public async Task<Role> AddAsync(Role role, Guid userId)
        {
            var application = await _applicationRepository.FindAsync(role.ApplicationId);
            if (application?.UserId != userId)
            {
                throw new ForbiddenException();
            }

            return await AddAsync(role, application);
        }

        private async Task<Role> AddAsync(Role role, Application application)
        {
            if (application.Roles.Any(r => r.Name == role.Name))
            {
                throw new ConflictException("A role with that name already exists in this application.");
            }

            _roleRepository.Add(role);
            await _roleRepository.SaveAsync();

            return role;
        }
    }
}
