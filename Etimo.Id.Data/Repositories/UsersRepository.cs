using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly EtimoIdDbContext _dbContext;

        public UsersRepository(EtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> ExistsByUsernameAsync(string username)
        {
            return _dbContext.Users.AnyAsync(u => u.Username == username);
        }

        public Task<bool> AnyAsync()
        {
            return _dbContext.Users.AnyAsync();
        }

        public ValueTask<User> FindAsync(Guid userId)
        {
            return _dbContext.Users.FindAsync(userId);
        }

        public Task<User> FindByUsernameAsync(string username)
        {
            return _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public User Add(User user)
        {
            return _dbContext.Users.Add(user).Entity;
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public Task<List<User>> GetAllAsync()
        {
            return _dbContext.Users.ToListAsync();
        }

        public async Task<bool> DeleteAsync(Guid userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                return true;
            }

            return true;
        }
    }
}
