using Etimo.Id.Abstractions;
using Etimo.Id.Api.Applications;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Roles;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
using Etimo.Id.Exceptions;
using Etimo.Id.Service.Scopes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Users
{
    [ApiController]
    public class UserController : Controller
    {
        private readonly IAddUserRoleRelationService    _addUserRoleRelationService;
        private readonly IAddUserService                _addUserService;
        private readonly IDeleteUserRoleRelationService _deleteUserRoleRelationService;
        private readonly IDeleteUserService             _deleteUserService;
        private readonly IFindUserService               _findUserService;
        private readonly IGetApplicationsService        _getApplicationsService;
        private readonly IGetRolesService               _getRolesService;
        private readonly IGetUsersService               _getUsersService;
        private readonly SiteSettings                   _siteSettings;
        private readonly IUnlockUserService             _unlockUserService;
        private readonly IUpdateUserService             _updateUserService;

        public UserController(
            SiteSettings siteSettings,
            IAddUserRoleRelationService addUserRoleRelationService,
            IAddUserService addUserService,
            IDeleteUserRoleRelationService deleteUserRoleRelationService,
            IDeleteUserService deleteUserService,
            IFindUserService findUserService,
            IGetApplicationsService getApplicationsService,
            IGetRolesService getRolesService,
            IGetUsersService getUsersService,
            IUnlockUserService unlockUserService,
            IUpdateUserService updateUserService)
        {
            _siteSettings                  = siteSettings;
            _addUserRoleRelationService    = addUserRoleRelationService;
            _addUserService                = addUserService;
            _deleteUserRoleRelationService = deleteUserRoleRelationService;
            _deleteUserService             = deleteUserService;
            _findUserService               = findUserService;
            _getApplicationsService        = getApplicationsService;
            _getRolesService               = getRolesService;
            _getUsersService               = getUsersService;
            _unlockUserService             = unlockUserService;
            _updateUserService             = updateUserService;
        }

        [HttpGet]
        [Route("/users")]
        [Authorize(Policy = UserScopes.Read)]
        public async Task<IActionResult> GetAsync()
        {
            List<User> users;
            if (this.UserHasScope(UserScopes.Admin)) { users = await _getUsersService.GetAllAsync(); }
            else
            {
                User user = await _findUserService.FindAsync(this.GetUserId());
                users = new List<User> { user };
            }

            IEnumerable<UserResponseDto> found = users.Select(UserResponseDto.FromUser);

            return Ok(found);
        }

        [HttpGet]
        [Route("/users/{userId:guid}")]
        [Authorize(Policy = UserScopes.Read)]
        public async Task<IActionResult> FindAsync([FromRoute] Guid userId)
        {
            User user;
            if (this.UserHasScope(UserScopes.Admin) || userId == this.GetUserId()) { user = await _findUserService.FindAsync(userId); }
            else { throw new ForbiddenException(); }

            var found = UserResponseDto.FromUser(user);

            return Ok(found);
        }

        [HttpGet]
        [Route("/users/{userId:guid}/applications")]
        [Authorize(Policy = CombinedScopes.ReadUserApplication)]
        public async Task<IActionResult> GetApplicationsAsync([FromRoute] Guid userId)
        {
            List<Application> applications;
            if (this.UserHasScope(UserScopes.Admin) || userId == this.GetUserId())
            {
                applications = await _getApplicationsService.GetByUserIdAsync(userId);
            }
            else { throw new ForbiddenException(); }

            IEnumerable<ApplicationResponseDto> found = applications.Select(a => ApplicationResponseDto.FromApplication(a, false));

            return Ok(found);
        }

        [HttpGet]
        [Route("/users/{userId:guid}/roles")]
        [Authorize(Policy = CombinedScopes.ReadUserRole)]
        public async Task<IActionResult> GetRolesAsync([FromRoute] Guid userId)
        {
            List<Role> roles;
            if (this.UserHasScope(UserScopes.Admin) || userId == this.GetUserId())
            {
                roles = await _getRolesService.GetByUserIdAsync(userId);
            }
            else { throw new ForbiddenException(); }

            IEnumerable<RoleResponseDto> found = roles.Select(a => RoleResponseDto.FromRole(a, false));

            return Ok(found);
        }

        [HttpPost]
        [Route("/users")]
        [ValidateModel]
        [Authorize(Policy = UserScopes.Admin)]
        public async Task<IActionResult> CreateAsync([FromBody] UserRequestDto createDto)
        {
            User user    = await _addUserService.AddAsync(createDto.ToUser());
            var  created = UserResponseDto.FromUser(user);

            return Created($"{_siteSettings.ListenUri}/users/{user.UserId}", created);
        }

        [HttpPut]
        [Route("/users/{userId:guid}")]
        [ValidateModel]
        [Authorize(Policy = UserScopes.Write)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid userId, [FromBody] UserRequestDto dto)
        {
            User user;
            if (this.UserHasScope(UserScopes.Admin) || userId == this.GetUserId())
            {
                user = await _updateUserService.UpdateAsync(dto.ToUser(userId));
            }
            else { throw new ForbiddenException(); }

            var updated = UserResponseDto.FromUser(user);

            return Ok(updated);
        }

        [HttpPut]
        [Route("/users/{userId:guid}/roles/{roleId:guid}")]
        [Authorize(Policy = UserScopes.Write)]
        public async Task<IActionResult> AddRoleRelationAsync([FromRoute] Guid userId, [FromRoute] Guid roleId)
        {
            List<Role> roles;
            if (this.UserHasScope(UserScopes.Admin) || userId == this.GetUserId())
            {
                roles = await _addUserRoleRelationService.AddRoleRelationAsync(userId, roleId);
            }
            else { throw new ForbiddenException(); }

            IEnumerable<RoleResponseDto> added = roles.Select(r => RoleResponseDto.FromRole(r, false));

            return Ok(added);
        }

        [HttpPut]
        [Route("/users/{userId:guid}/unlock")]
        [Authorize(Policy = UserScopes.Write)]
        public async Task<IActionResult> UnlockAsync([FromRoute] Guid userId)
        {
            if (this.UserHasScope(UserScopes.Admin) || userId == this.GetUserId()) { await _unlockUserService.UnlockAsync(userId); }
            else { throw new ForbiddenException(); }

            return Ok();
        }

        [HttpDelete]
        [Route("/users/{userId:guid}")]
        [Authorize(Policy = UserScopes.Admin)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid userId)
        {
            await _deleteUserService.DeleteAsync(userId);

            return NoContent();
        }

        [HttpDelete]
        [Route("/users/{userId:guid}/roles/{roleId:guid}")]
        [Authorize(Policy = UserScopes.Write)]
        public async Task<IActionResult> DeleteRoleRelationAsync([FromRoute] Guid userId, [FromRoute] Guid roleId)
        {
            List<Role> roles;
            if (this.UserHasScope(UserScopes.Admin) || userId == this.GetUserId())
            {
                roles = await _deleteUserRoleRelationService.DeleteRoleRelationAsync(userId, roleId);
            }
            else { throw new ForbiddenException(); }

            IEnumerable<RoleResponseDto> remaining = roles.Select(r => RoleResponseDto.FromRole(r, false));

            return Ok(remaining);
        }
    }
}
