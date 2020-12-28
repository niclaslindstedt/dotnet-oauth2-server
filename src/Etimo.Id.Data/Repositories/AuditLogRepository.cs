using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Data.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly IEtimoIdDbContext _dbContext;

        public AuditLogRepository(IEtimoIdDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<AuditLog>> GetAllAsync()
            => _dbContext.AuditLogs.ToListAsync();

        public Task<List<AuditLog>> GetByUserIdAsync(Guid userId)
            => _dbContext.AuditLogs.Where(a => a.UserId == userId).ToListAsync();

        public Task<List<AuditLog>> GetByApplicationIdAsync(int applicationId)
            => _dbContext.AuditLogs.Where(a => a.ApplicationId == applicationId).ToListAsync();

        public Task<AuditLog> FindAsync(int auditLogId)
            => _dbContext.AuditLogs.FindAsync(auditLogId).AsTask();

        public void Add(AuditLog token)
            => _dbContext.AuditLogs.Add(token);

        public void Delete(AuditLog token)
        {
            if (token != null) { _dbContext.AuditLogs.Remove(token); }
        }

        public Task<int> SaveAsync()
            => _dbContext.SaveChangesAsync();
    }
}
