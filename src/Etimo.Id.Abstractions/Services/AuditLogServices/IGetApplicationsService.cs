using Etimo.Id.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Abstractions
{
    public interface IGetAuditLogsService
    {
        Task<List<AuditLog>> GetAllAsync();
        Task<List<AuditLog>> GetByUserIdAsync(Guid userId);
    }
}
