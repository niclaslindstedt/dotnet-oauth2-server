using Etimo.Id.Entities;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IFindAuditLogService
    {
        Task<AuditLog> FindAsync(int auditLogId);
        Task<AuditLog> FindAsync(int auditLogId, Guid userId);
    }
}
