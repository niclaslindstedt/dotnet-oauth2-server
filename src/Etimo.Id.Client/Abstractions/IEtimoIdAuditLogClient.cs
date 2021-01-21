using Etimo.Id.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public interface IEtimoIdAuditLogClient
    {
        Task<ResponseDto<List<AuditLogResponseDto>>> GetAuditLogsAsync();
        Task<ResponseDto<AuditLogResponseDto>> GetAuditLogAsync(int auditLogId);
    }
}
