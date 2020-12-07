using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        ValueTask<User> FindAsync(Guid userId);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User updatedUser);
        Task<User> UpdateAsync(User updatedUser, Guid userId);
        Task DeleteAsync(Guid userId);
        Task<bool> AnyAsync();
        Task<User> AuthenticateAsync(string username, string password);
    }
}