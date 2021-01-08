using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IFindUserService
    {
        Task<User> FindAsync(Guid userId);
    }
}
