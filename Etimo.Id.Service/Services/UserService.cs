using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using Microsoft.EntityFrameworkCore;
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

        public Task<bool> AnyAsync()
        {
            return _userRepository.AnyAsync();
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

        public Task<List<User>> GetAllAsync()
        {
            return _userRepository.GetAllAsync();
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
                throw new BadRequestException("User does not exist or does not belong to you.");
            }

            return UpdateAsync(updatedUser);
        }

        public async Task<User> AddAsync(User user)
        {
            // We assume the password is not encrypted at this point.
            user.Password = _passwordHasher.Hash(user.Password);

            try
            {
                _userRepository.Add(user);
                await _userRepository.SaveAsync();
            }
            catch (DbUpdateException)
            {
                throw new ConflictException("Username or e-mail already taken.");
            }

            return user;
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

        public async Task DeleteAsync(Guid userId)
        {
            var user = await _userRepository.FindAsync(userId);
            if (user != null)
            {
                _userRepository.Delete(user);
                await _userRepository.SaveAsync();
            }
        }
    }
}
