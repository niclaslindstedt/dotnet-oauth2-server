using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etimo.Id.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid RefreshTokenId { get; set; } = Guid.NewGuid();
        public DateTime ExpirationDate { get; set; }
        public string Scope { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(Application))]
        public int ApplicationId { get; set; }

        public virtual Application Application { get; set; }

        public bool IsExpired => ExpirationDate < DateTime.UtcNow;
    }
}
