using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync();
        Task<Role> FindAsync(Guid roleId);
        void Add(Role role);
        void Delete(Role role);
        Task<int> SaveAsync();
    }
}
