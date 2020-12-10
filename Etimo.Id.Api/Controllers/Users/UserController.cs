using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
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
        private readonly SiteSettings _siteSettings;
        private readonly IAddUserService _addUserService;
        private readonly IDeleteUserService _deleteUserService;
        private readonly IFindUserService _findUserService;
        private readonly IGetUsersService _getUsersService;
        private readonly IUpdateUserService _updateUserService;

        public UserController(
            SiteSettings siteSettings,
            IAddUserService addUserService,
            IDeleteUserService deleteUserService,
            IFindUserService findUserService,
            IGetUsersService getUsersService,
            IUpdateUserService updateUserService)
        {
            _siteSettings = siteSettings;
            _addUserService = addUserService;
            _deleteUserService = deleteUserService;
            _findUserService = findUserService;
            _getUsersService = getUsersService;
            _updateUserService = updateUserService;
        }

        [HttpGet]
        [Route("/users")]
        [Authorize(Policy = UserScopes.Read)]
        public async Task<IActionResult> GetAsync()
        {
            List<User> users;
            if (this.UserHasScope(UserScopes.Admin))
            {
                users = await _getUsersService.GetAllAsync();
            }
            else
            {
                var user = await _findUserService.FindAsync(this.GetUserId());
                users = new List<User> {user};
            }

            var found = users.Select(UserResponseDto.FromUser);

            return Ok(found);
        }

        [HttpGet]
        [Route("/users/{userId:guid}")]
        [Authorize(Policy = UserScopes.Read)]
        public async Task<IActionResult> FindAsync([FromRoute] Guid userId)
        {
            User user;
            if (this.UserHasScope(UserScopes.Admin))
            {
                user = await _findUserService.FindAsync(userId);
            }
            else
            {
                user = await _findUserService.FindAsync(this.GetUserId());
            }

            var found = UserResponseDto.FromUser(user);

            return Ok(found);
        }

        [HttpPost]
        [Route("/users")]
        [ValidateModel]
        [Authorize(Policy = UserScopes.Admin)]
        public async Task<IActionResult> CreateAsync([FromBody] UserRequestDto createDto)
        {
            var user = await _addUserService.AddAsync(createDto.ToUser());
            var created = UserResponseDto.FromUser(user);

            return Created($"{_siteSettings.ListenUri}/users/{user.UserId}", created);
        }

        [HttpPut]
        [Route("/users/{userId:guid}")]
        [ValidateModel]
        [Authorize(Policy = UserScopes.Write)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid userId, [FromBody] UserRequestDto dto)
        {
            User user;
            if (this.UserHasScope(UserScopes.Admin))
            {
                user = await _updateUserService.UpdateAsync(dto.ToUser(userId));
            }
            else
            {
                user = await _updateUserService.UpdateAsync(dto.ToUser(userId), this.GetUserId());
            }

            var updated = UserResponseDto.FromUser(user);

            return Ok(updated);
        }

        [HttpDelete]
        [Route("/users/{userId:guid}")]
        [Authorize(Policy = UserScopes.Admin)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid userId)
        {
            await _deleteUserService.DeleteAsync(userId);

            return NoContent();
        }
    }
}
