using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IUsersRepository
    {
        Task<bool> AnyAsync();
        ValueTask<User> FindAsync(Guid userId);
        Task<User> FindByUsernameAsync(string username);
        User Add(User user);
        Task<int> SaveAsync();
        Task<List<User>> GetAllAsync();
        void Delete(User user);
    }
}
