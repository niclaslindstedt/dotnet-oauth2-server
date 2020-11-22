using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IUsersService
    {
        Task<List<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        ValueTask<User> FindAsync(Guid userId);
        Task DeleteAsync(Guid userId);
        Task<bool> ExistsAsync(string username);
        Task<User> AuthenticateAsync(string username, string password);
    }
}
