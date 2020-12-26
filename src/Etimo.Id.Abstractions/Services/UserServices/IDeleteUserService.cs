using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IDeleteUserService
    {
        Task DeleteAsync(Guid userId);
    }
}
