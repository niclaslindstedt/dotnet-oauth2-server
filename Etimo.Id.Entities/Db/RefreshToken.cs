using System;

namespace Etimo.Id.Entities
{
    public class RefreshToken
    {
        public string RefreshTokenId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Scope { get; set; }
        public string RedirectUri { get; set; }
        public Guid? AccessTokenId { get; set; }
        public Guid UserId { get; set; }
        public int ApplicationId { get; set; }
        public bool Used { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }
        public virtual Application Application { get; set; }
        public virtual AccessToken AccessToken { get; set; }

        public bool IsExpired => ExpirationDate < DateTime.UtcNow;
    }
}
