using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etimo.Id.Entities
{
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; } = Guid.NewGuid();
        public DateTime ExpirationDate { get; set; }
        public string Scope { get; set; }
        public Guid UserId { get; set; }
        public int ApplicationId { get; set; }

        public virtual User User { get; set; }
        public virtual Application Application { get; set; }

        public bool IsExpired => ExpirationDate < DateTime.UtcNow;
    }
}
