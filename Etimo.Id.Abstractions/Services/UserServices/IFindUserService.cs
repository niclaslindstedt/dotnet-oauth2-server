using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IFindUserService
    {
        ValueTask<User> FindAsync(Guid userId);
    }
}
