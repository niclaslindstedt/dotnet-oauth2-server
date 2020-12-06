using Etimo.Id.Abstractions;
using Etimo.Id.Api.Attributes;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Api.Settings;
using Etimo.Id.Service.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
            if (this.UserHasScope(UserScopes.Admin))
            {
                var users = await _userService.GetAllAsync();
                var userDtos = users.Select(UserResponseDto.FromUser);

                return Ok(userDtos);
            }

            return await FindAsync(this.GetUserId());
        }

        [HttpPost]
        [Route("/users")]
        [ValidateModel]
        public async Task<IActionResult> CreateAsync([FromBody] UserRequestDto createDto)
        {
            // This method will allow a user to use a super admin key to
            // authenticate when the database is empty.
            await AuthorizeAsync();

            var user = await _userService.AddAsync(createDto.ToUser());

            return Created($"{_siteSettings.ListenUri}/users/{user.UserId}", UserResponseDto.FromUser(user));
        }

        [HttpGet]
        [Route("/users/{userId:guid}")]
        [Authorize(Policy = UserScopes.Read)]
        public async Task<IActionResult> FindAsync([FromRoute] Guid userId)
        {
            // If the caller is not an admin, only allow the user to fetch itself.
            if (!this.UserHasScope(UserScopes.Admin))
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
        [Authorize(Policy = UserScopes.Admin)]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid userId)
        {
            await _userService.DeleteAsync(userId);

            return NoContent();
        }

        private async Task AuthorizeAsync()
        {
            // The super admin key can be used to create the first user with administrator privileges.
            // Set the key using: dotnet user-secrets set SiteSettings:SuperAdminKey 'key'
            if (this.SuperAdminKeyHeader() == _siteSettings.SuperAdminKey)
            {
                if (await _userService.AnyAsync())
                {
                    throw new BadRequestException("The SA key is only valid when database is empty.");
                }
            }

            if (!this.UserIsAuthenticated())
            {
                throw new ForbiddenException();
            }

            if (!this.UserHasScope(UserScopes.Admin))
            {
                throw new ForbiddenException();
            }
        }
    }
}
