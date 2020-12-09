using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Settings;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
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
        private readonly IUserService _userService;

        public UserController(
            SiteSettings siteSettings,
            IUserService userService)
        {
            _siteSettings = siteSettings;
            _userService = userService;
        }

        [HttpGet]
        [Route("/users")]
        [Authorize(Policy = UserScopes.Read)]
        public async Task<IActionResult> GetAsync()
        {
            List<User> users;
            if (this.UserHasScope(UserScopes.Admin))
            {
                users = await _userService.GetAllAsync();
            }
            else
            {
                var user = await _userService.FindAsync(this.GetUserId());
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
                user = await _userService.FindAsync(userId);
            }
            else
            {
                user = await _userService.FindAsync(this.GetUserId());
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
            var user = await _userService.AddAsync(createDto.ToUser());
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
                user = await _userService.UpdateAsync(dto.ToUser(userId));
            }
            else
            {
                user = await _userService.UpdateAsync(dto.ToUser(userId), this.GetUserId());
            }

            var updated = UserResponseDto.FromUser(user);

            return Ok(updated);
        }

        [HttpDelete]
        [Route("/users/{userId:guid}")]
        [Authorize(Policy = UserScopes.Admin)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid userId)
        {
            await _userService.DeleteAsync(userId);

            return NoContent();
        }
    }
}
