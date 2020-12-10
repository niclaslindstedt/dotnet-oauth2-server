using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public Task<List<User>> GetAllAsync()
        {
            return _userRepository.GetAllAsync();
        }

        public async ValueTask<User> FindAsync(Guid userId)
        {
            var user = await _userRepository.FindAsync(userId);
            if (user == null)
            {
                throw new NotFoundException();
            }

            return user;
        }

        public async Task<User> AddAsync(User user)
        {
            if (await _userRepository.AnyByUsernameAsync(user.Username))
            {
                throw new ConflictException("Username already exists.");
            }

            // We assume the password is not encrypted at this point.
            user.Password = _passwordHasher.Hash(user.Password);

            _userRepository.Add(user);
            await _userRepository.SaveAsync();

            return user;
        }

        public async Task<User> UpdateAsync(User updatedUser)
        {
            var user = await _userRepository.FindAsync(updatedUser.UserId);

            user.Update(updatedUser);
            await _userRepository.SaveAsync();

            return user;
        }

        public Task<User> UpdateAsync(User updatedUser, Guid userId)
        {
            if (updatedUser.UserId != userId)
            {
                throw new BadRequestException("You can only update your own user.");
            }

            return UpdateAsync(updatedUser);
        }

        public async Task DeleteAsync(Guid userId)
        {
            var user = await _userRepository.FindAsync(userId);
            if (user != null)
            {
                _userRepository.Delete(user);
                await _userRepository.SaveAsync();
            }
        }

        public Task<bool> AnyAsync()
        {
            return _userRepository.AnyAsync();
        }

        public Task<bool> AnyAsync(Guid userId)
        {
            return _userRepository.AnyAsync(userId);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.FindByUsernameAsync(username);
            if (user == null)
            {
                throw new InvalidGrantException("Invalid user credentials.");
            }

            if (!_passwordHasher.Verify(password, user.Password))
            {
                throw new InvalidGrantException("Invalid user credentials.");
            }

            return user;
        }
    }
}
