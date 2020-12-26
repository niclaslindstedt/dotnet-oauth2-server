using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using Etimo.Id.Service.Exceptions;
using System;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class FindAuditLogService : IFindAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public FindAuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task<AuditLog> FindAsync(int auditLogId)
        {
            var auditLog = await _auditLogRepository.FindAsync(auditLogId);
            if (auditLog == null)
            {
                throw new NotFoundException();
            }

            return auditLog;
        }

        public async Task<AuditLog> FindAsync(int auditLogId, Guid userId)
        {
            var auditLog = await _auditLogRepository.FindAsync(auditLogId);
            if (auditLog?.UserId != userId)
            {
                throw new NotFoundException();
            }

            return auditLog;
        }
    }
}
