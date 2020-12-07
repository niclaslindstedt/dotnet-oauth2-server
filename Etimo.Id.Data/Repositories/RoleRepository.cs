using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task<List<Role>> GetByUserIdAsync(Guid userId)
        {
            return _dbContext.Roles.Where(r => r.Application.UserId == userId).ToListAsync();
        }

        public Task<Role> FindAsync(Guid roleId)
        {
            return _dbContext.Roles.FindAsync(roleId).AsTask();
        }

        public void Add(Role role)
        {
            _dbContext.Roles.Add(role);
        }

        public void Delete(Role role)
        {
            if (role != null)
            {
                _dbContext.Roles.Remove(role);
            }
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
    }
}
