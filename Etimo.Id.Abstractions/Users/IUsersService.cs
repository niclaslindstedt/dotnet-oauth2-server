using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IUsersService
    {
        Task<bool> ExistsAsync(IUser user);
        Task<bool> ExistsAsync(string username);
        Task ValidateUserPassword(string username, string password);
        Task<User> AddAsync(User user);
        Task<List<User>> GetAllAsync();
        Task DeleteAsync(Guid userId);
    }
}
