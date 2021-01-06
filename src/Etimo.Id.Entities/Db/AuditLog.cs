using System;

namespace Etimo.Id.Entities
{
    public class AuditLog
    {
        public int      AuditLogId      { get; set; }
        public string   Type            { get; set; }
        public string   Message         { get; set; }
        public string   Body            { get; set; }
        public Guid?    UserId          { get; set; }
        public int      ApplicationId   { get; set; }
        public string   IpAddress       { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        public virtual User        User        { get; set; }
        public virtual Application Application { get; set; }
    }
}
