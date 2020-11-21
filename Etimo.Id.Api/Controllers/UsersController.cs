using Etimo.Id.Abstractions;
using Etimo.Id.Api.Models;
using Etimo.Id.Security;
using Etimo.Id.Service.Exceptions;
using Etimo.Id.Service.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        [Authorize(Policy = Policies.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _usersService.GetAllAsync();
            var userDtos = users.Select(UserDto.FromUser);

            return Ok(userDtos);
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
