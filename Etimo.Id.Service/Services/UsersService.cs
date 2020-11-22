using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger _logger;

        public UsersService(
            IUsersRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public Task<bool> ExistsAsync(string username)
        {
            return _userRepository.ExistsByUsernameAsync(username);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            if (user == null)
            {
                throw new BadRequestException("invalid_grant");
            }

            if (!_passwordHasher.Verify(password, user.Password))
            {
                throw new BadRequestException("invalid_grant");
            }

            return user;
        }

        public Task<List<User>> GetAllAsync()
        {
            return _userRepository.GetAllAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            _userRepository.Add(user);
            await _userRepository.SaveAsync();
            return user;
        }

        public ValueTask<User> FindAsync(Guid userId)
        {
            return _userRepository.FindAsync(userId);
        }

        public async Task DeleteAsync(Guid userId)
        {
            if (await _userRepository.DeleteAsync(userId))
            {
                await _userRepository.SaveAsync();
            }
        }
    }
}
