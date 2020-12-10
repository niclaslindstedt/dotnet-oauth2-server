using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IUpdateUserService
    {
        Task<User> UpdateAsync(User updatedUser);
    }
}
