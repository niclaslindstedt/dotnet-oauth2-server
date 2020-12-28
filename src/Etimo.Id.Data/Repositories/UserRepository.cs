using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IEtimoIdDbContext _dbContext;

        public UserRepository(IEtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<User>> GetAllAsync()
            => _dbContext.Users.ToListAsync();

        public Task<User> FindAsync(Guid userId)
            => _dbContext.Users.FindAsync(userId).AsTask();

        public Task<User> FindByUsernameAsync(string username)
            => _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

        public void Add(User user)
            => _dbContext.Users.Add(user);

        public void Delete(User user)
            => _dbContext.Users.Remove(user);

        public Task<int> SaveAsync()
            => _dbContext.SaveChangesAsync();

        public Task<bool> AnyAsync()
            => _dbContext.Users.AnyAsync();

        public Task<bool> AnyAsync(Guid userId)
            => _dbContext.Users.AnyAsync(u => u.UserId == userId);

        public Task<bool> AnyByUsernameAsync(string username)
            => _dbContext.Users.AnyAsync(u => u.Username == username);
    }
}
