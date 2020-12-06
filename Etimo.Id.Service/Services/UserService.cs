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
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(
            IUsersRepository usersRepository,
            IPasswordHasher passwordHasher)
        {
            _usersRepository = usersRepository;
            _passwordHasher = passwordHasher;
        }

        public Task<bool> AnyAsync()
        {
            return _usersRepository.AnyAsync();
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _usersRepository.FindByUsernameAsync(username);
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
            return _usersRepository.GetAllAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            // We assume the password is not encrypted at this point.
            user.Password = _passwordHasher.Hash(user.Password);

            try
            {
                _usersRepository.Add(user);
                await _usersRepository.SaveAsync();
            }
            catch (DbUpdateException)
            {
                throw new ConflictException("Username or e-mail already taken.");
            }

            return user;
        }

        public async ValueTask<User> FindAsync(Guid userId)
        {
            var user = await _usersRepository.FindAsync(userId);
            if (user == null)
            {
                throw new NotFoundException();
            }

            return user;
        }

        public async Task DeleteAsync(Guid userId)
        {
            var user = await _usersRepository.FindAsync(userId);
            if (user != null)
            {
                _usersRepository.Delete(user);
                await _usersRepository.SaveAsync();
            }
        }
    }
}
