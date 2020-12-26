using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IAuditLogRepository
    {
        Task<List<AuditLog>> GetAllAsync();
        Task<List<AuditLog>> GetByUserIdAsync(Guid userId);
        Task<List<AuditLog>> GetByApplicationIdAsync(int applicationId);
        Task<AuditLog> FindAsync(int auditLogId);
        void Add(AuditLog auditLog);
        void Delete(AuditLog auditLog);
        Task<int> SaveAsync();
    }
}
