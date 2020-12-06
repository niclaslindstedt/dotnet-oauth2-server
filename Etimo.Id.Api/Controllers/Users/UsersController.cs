using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Security;
using Etimo.Id.Api.Settings;
using Etimo.Id.Service.Constants;
using Etimo.Id.Service.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Users
{
    [ApiController]
    public class UsersController : Controller
    {
        private readonly SiteSettings _siteSettings;
        private readonly IUserService _userService;

        public UsersController(
            SiteSettings siteSettings,
            IUserService userService)
        {
            _siteSettings = siteSettings;
            _userService = userService;
        }

        [HttpGet]
        [Route("/users")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> GetAsync()
        {
            // If the caller is not an admin, revert to the FindAsync method.
            if (!this.UserHasRole(Roles.Admin))
            {
                return await FindAsync(this.GetUserId());
            }

            var users = await _userService.GetAllAsync();
            var userDtos = users.Select(UserResponseDto.FromUser);

            return Ok(userDtos);
        }

        [HttpPost]
        [Route("/users")]
        [ValidateModel]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> CreateAsync([FromBody] UserRequestDto createDto)
        {
            await CheckPresenceOfSuperAdminKeyAsync();

            var user = await _userService.AddAsync(createDto.ToUser());

            return Created($"{_siteSettings.ListenUri}/users/{user.UserId}", UserResponseDto.FromUser(user));
        }

        [HttpGet]
        [Route("/users/{userId:guid}")]
        [Authorize(Policy = Policies.User)]
        public async Task<IActionResult> FindAsync([FromRoute] Guid userId)
        {
            // If the caller is not an admin, only allow the user to fetch itself.
            if (!this.UserHasRole(Roles.Admin))
            {
                var identityId = this.GetUserId();
                if (userId != identityId)
                {
                    throw new ForbiddenException();
                }
            }

            var user = await _userService.FindAsync(userId);

            return Ok(UserResponseDto.FromUser(user));
        }

        [HttpDelete]
        [Route("/users/{userId:guid}")]
        [Authorize(Policy = Policies.Admin)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid userId)
        {
            await _userService.DeleteAsync(userId);

            return NoContent();
        }

        private async Task CheckPresenceOfSuperAdminKeyAsync()
        {
            // The Super Admin Key can be used to create the first user with administrator privileges.
            // Set the key using: dotnet user-secrets set SiteSettings:SuperAdminKey 'key'
            if (this.SuperAdminKeyHeader() != _siteSettings.SuperAdminKey)
            {
                if (this.UserIsAuthenticated())
                {
                    throw new ForbiddenException();
                }

                if (!this.UserHasRole(Roles.Admin))
                {
                    throw new UnauthorizedException();
                }

                if (await _userService.AnyAsync())
                {
                    throw new BadRequestException("The SA key is only valid when database is empty.");
                }
            }
        }
    }
}
