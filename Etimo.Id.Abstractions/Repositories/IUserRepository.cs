using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User> FindAsync(Guid userId);
        Task<User> FindByUsernameAsync(string username);
        void Add(User user);
        void Delete(User user);
        Task<int> SaveAsync();
        Task<bool> AnyAsync();
    }
}
