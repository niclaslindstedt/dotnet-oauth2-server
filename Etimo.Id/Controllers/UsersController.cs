using System.Collections.Generic;
using System.Linq;
using Etimo.Id.Entities;
using Etimo.Id.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Etimo.Id.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly ILogger<OAuthController> _logger;
        private readonly EtimoIdDbContext _context;

        public UsersController(
            ILogger<OAuthController> logger,
            EtimoIdDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpPost]
        public async Task Create(UserDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        [Authorize(Policy = Policies.Admin)]
        [HttpGet]
        public async Task<List<UserDto>> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            var userDtos = users.Select(user => new UserDto
            {
                Username = user.Username,
                Password = "hidden"
            }).ToList();

            return userDtos;
        }
    }
}
