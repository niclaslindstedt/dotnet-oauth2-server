using Etimo.Id.Abstractions;
using Etimo.Id.Api.Helpers;
using Etimo.Id.Entities;
using Etimo.Id.Service.Scopes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etimo.Id.Api.Applications
{
    [ApiController]
    public class AuditLogController : Controller
    {
        private readonly IFindAuditLogService _findAuditLogService;
        private readonly IGetAuditLogsService _getAuditLogsService;

        public AuditLogController(IFindAuditLogService findAuditLogService, IGetAuditLogsService getAuditLogsService)
        {
            _findAuditLogService = findAuditLogService;
            _getAuditLogsService = getAuditLogsService;
        }

        [HttpGet]
        [Route("/auditlogs")]
        [Authorize(Policy = AuditLogScopes.Admin)]
        public async Task<IActionResult> GetAsync()
        {
            List<AuditLog> auditLogs;
            if (this.UserHasScope(AuditLogScopes.Admin)) { auditLogs = await _getAuditLogsService.GetAllAsync(); }
            else { auditLogs                                         = await _getAuditLogsService.GetByUserIdAsync(this.GetUserId()); }

            IEnumerable<AuditLogResponseDto> found = auditLogs.Select(AuditLogResponseDto.FromAuditLog);

            return Ok(found);
        }

        [HttpGet]
        [Route("/auditlogs/{auditLogId:int}")]
        [Authorize(Policy = AuditLogScopes.Read)]
        public async Task<IActionResult> FindAsync([FromRoute] int auditLogId)
        {
            AuditLog auditLog;
            if (this.UserHasScope(AuditLogScopes.Admin)) { auditLog = await _findAuditLogService.FindAsync(auditLogId); }
            else { auditLog                                         = await _findAuditLogService.FindAsync(auditLogId, this.GetUserId()); }

            var found = AuditLogResponseDto.FromAuditLog(auditLog);

            return Ok(found);
        }
    }
}
