using Etimo.Id.Entities;
using Etimo.Id.Exceptions;
using Etimo.Id.Models;
using Etimo.Id.Security;
using Etimo.Id.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly SiteSettings _siteSettings;
        private readonly EtimoIdDbContext _context;

        public UsersController(
            SiteSettings siteSettings,
            EtimoIdDbContext context)
        {
            _siteSettings = siteSettings;
            _context = context;
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateUserDto createDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == createDto.Username))
            {
                throw new ConflictException("Username already exists");
            }

            var user = MapUser(createDto);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var createdDto = MapUserDto(user);

            return Created($"{_siteSettings.ListenUri}/users/{user.UserId}", createdDto);
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _context.Users.ToListAsync();
            var userDtos = users.Select(MapUserDto).ToList();

            return Ok(userDtos);
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpDelete]
        [Route("{userId:guid}")]
        public async Task DeleteAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        private static User MapUser(CreateUserDto createDto)
        {
            return new User
            {
                Username = createDto.Username,
                Password = createDto.Password.BCrypt()
            };
        }

        private static UserDto MapUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username
            };
        }
    }
}
