using System;

namespace Etimo.Id.Entities
{
    public class AuthorizationCode
    {
        public Guid AuthorizationCodeId { get; set; } = Guid.NewGuid();
        public DateTime ExpirationDate { get; set; }
        public Guid ClientId { get; set; }

        public bool IsExpired => ExpirationDate < DateTime.UtcNow;
    }
}
