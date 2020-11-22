using Etimo.Id.Abstractions;
using Etimo.Id.Api.Models;
using Etimo.Id.Security;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Service.Constants;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Etimo.Id.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly SiteSettings _siteSettings;
        private readonly IUsersService _usersService;

        public UsersController(
            SiteSettings siteSettings,
            IUsersService usersService)
        {
            _siteSettings = siteSettings;
            _usersService = usersService;
        }

        [Authorize(Policy = Policies.User)]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            // If the user calling is not an admin, revert to the FindAsync method.
            if (!this.UserHasRole(Roles.Admin))
            {
                return await FindAsync(this.GetUserId());
            }

            var users = await _usersService.GetAllAsync();
            var userDtos = users.Select(UserDto.FromUser);

            return Ok(userDtos);
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateUserDto createDto)
        {
            if (await _usersService.ExistsAsync(createDto.Username))
            {
                throw new ConflictException("Username already exists");
            }

            var user = await _usersService.AddAsync(createDto.ToUser());

            return Created($"{_siteSettings.ListenUri}/users/{user.UserId}", UserDto.FromUser(user));
        }

        [Authorize(Policy = Policies.User)]
        [HttpGet]
        [Route("{userId:guid}")]
        public async Task<IActionResult> FindAsync(Guid userId)
        {
            // If the user calling is not an admin, only allow the user to fetch itself.
            if (!this.UserHasRole(Roles.Admin))
            {
                var identityId = this.GetUserId();
                if (userId != identityId)
                {
                    throw new ForbiddenException();
                }
            }

            var user = await _usersService.FindAsync(userId);

            return Ok(UserDto.FromUser(user));
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpDelete]
        [Route("{userId:guid}")]
        public Task DeleteAsync(Guid userId)
        {
            return _usersService.DeleteAsync(userId);
        }
    }
}
