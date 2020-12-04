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
        private readonly IEtimoIdDbContext _dbContext;

        public UsersRepository(IEtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<bool> AnyAsync()
        {
            return _dbContext.Users.AnyAsync();
        }

        public Task<User> FindAsync(Guid userId)
        {
            return _dbContext.Users.FindAsync(userId).AsTask();
        }

        public Task<User> FindByUsernameAsync(string username)
        {
            return _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public void Add(User user)
        {
            _dbContext.Users.Add(user);
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public Task<List<User>> GetAllAsync()
        {
            return _dbContext.Users.ToListAsync();
        }

        public void Delete(User user)
        {
            _dbContext.Users.Remove(user);
        }
    }
}
