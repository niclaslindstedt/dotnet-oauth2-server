using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class DeleteUserRoleRelationService : IDeleteUserRoleRelationService
    {
        private readonly IFindUserService _findUserService;
        private readonly IRoleRepository  _roleRepository;
        private readonly IUserRepository  _userRepository;

        public DeleteUserRoleRelationService(
            IFindUserService findUserService,
            IRoleRepository roleRepository,
            IUserRepository userRepository)
        {
            _findUserService = findUserService;
            _roleRepository  = roleRepository;
            _userRepository  = userRepository;
        }

        public async Task<List<Role>> DeleteRoleRelationAsync(Guid userId, Guid roleId)
        {
            User user = await _findUserService.FindAsync(userId);
            if (user.Roles.Any(s => s.RoleId == roleId))
            {
                Role role = await _roleRepository.FindAsync(roleId);
                if (role == null) { throw new NotFoundException(); }

                user.Roles.Remove(role);
                await _userRepository.SaveAsync();
            }

            return user.Roles.ToList();
        }
    }
}
