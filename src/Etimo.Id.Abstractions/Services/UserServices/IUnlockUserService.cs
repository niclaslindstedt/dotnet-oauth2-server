using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IUnlockUserService
    {
        Task UnlockAsync(Guid userId);
    }
}
