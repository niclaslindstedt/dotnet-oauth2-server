using Etimo.Id.Entities;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface ILockUserService
    {
        Task LockAsync(User user);
    }
}
