using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IEtimoIdDbContext _dbContext;

        public RoleRepository(IEtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Role>> GetAllAsync()
        {
            return _dbContext.Roles.ToListAsync();
        }

        public Task<Role> FindAsync(Guid roleId)
        {
            return _dbContext.Roles.FindAsync(roleId).AsTask();
        }

        public void Add(Role role)
        {
            _dbContext.Roles.Add(role);
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Delete(Role role)
        {
            if (role != null)
            {
                _dbContext.Roles.Remove(role);
            }
        }
    }
}
