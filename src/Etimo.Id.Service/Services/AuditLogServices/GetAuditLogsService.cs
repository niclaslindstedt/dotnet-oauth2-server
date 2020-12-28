using Etimo.Id.Abstractions;
using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Service
{
    public class GetAuditLogsService : IGetAuditLogsService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public GetAuditLogsService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public Task<List<AuditLog>> GetAllAsync()
            => _auditLogRepository.GetAllAsync();

        public Task<List<AuditLog>> GetByUserIdAsync(Guid userId)
            => _auditLogRepository.GetByUserIdAsync(userId);
    }
}
