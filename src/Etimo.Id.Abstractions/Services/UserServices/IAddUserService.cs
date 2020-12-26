using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAddUserService
    {
        Task<User> AddAsync(User user);
    }
}
