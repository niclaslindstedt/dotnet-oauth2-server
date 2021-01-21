using Etimo.Id.Dtos;
using Etimo.Id.Settings;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Etimo.Id.Client
{
    public class EtimoIdAuditLogClient
        : EtimoIdBaseClient,
            IEtimoIdAuditLogClient
    {
        public EtimoIdAuditLogClient(EtimoIdSettings settings, HttpClient httpClient)
            : base(settings, httpClient) { }

        public Task<ResponseDto<List<AuditLogResponseDto>>> GetAuditLogsAsync()
            => GetAsync<List<AuditLogResponseDto>>("/auditlogs");

        public Task<ResponseDto<AuditLogResponseDto>> GetAuditLogAsync(int auditLogId)
            => GetAsync<AuditLogResponseDto>($"/auditlogs/{auditLogId}");
    }
}
