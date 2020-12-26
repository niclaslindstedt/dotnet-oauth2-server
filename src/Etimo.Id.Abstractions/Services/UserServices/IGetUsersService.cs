using Etimo.Id.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IGetUsersService
    {
        Task<List<User>> GetAllAsync();
    }
}
