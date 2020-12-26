using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class AddUserRoleRelationService : IAddUserRoleRelationService
    {
        private readonly IFindUserService _findUserService;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        public AddUserRoleRelationService(
            IFindUserService findUserService,
            IRoleRepository roleRepository,
            IUserRepository userRepository)
        {
            _findUserService = findUserService;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Role>> AddRoleRelationAsync(Guid userId, Guid roleId)
        {
            var user = await _findUserService.FindAsync(userId);
            if (!user.Roles.Any(s => s.RoleId == roleId))
            {
                var role = await _roleRepository.FindAsync(roleId);
                if (role == null || role.Application.UserId != userId)
                {
                    throw new NotFoundException();
                }

                user.Roles.Add(role);
                await _userRepository.SaveAsync();
            }

            return user.Roles.ToList();
        }
    }
}
