// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;
using System;
using System.Text.Json;

namespace Etimo.Id.Api.Applications
{
    public class AuditLogResponseDto
    {
        public int audit_log_id { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public object body { get; set; }
        public Guid user_id { get; set; }
        public int application_id { get; set; }
        public DateTime created_date { get; set; }

        public static AuditLogResponseDto FromAuditLog(AuditLog auditLog) => FromAuditLog(auditLog, true);
        public static AuditLogResponseDto FromAuditLog(AuditLog auditLog, bool includeChildren)
        {
            var dto = new AuditLogResponseDto
            {
                audit_log_id = auditLog.AuditLogId,
                type = auditLog.Type,
                message = auditLog.Message,
                body = JsonSerializer.Deserialize<object>(auditLog.Body),
                user_id = auditLog.UserId,
                application_id = auditLog.ApplicationId,
                created_date = auditLog.CreatedDateTime,
            };

            return dto;
        }
    }
}
