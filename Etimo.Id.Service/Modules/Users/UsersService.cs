using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service.Users
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UsersService(
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
            if (await _usersRepository.DeleteAsync(userId))
            {
                await _usersRepository.SaveAsync();
            }
        }
    }
}
