using System;

namespace Etimo.Id.Entities
{
    public class AuthorizationCode
    {
        public string Code { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool Used { get; set; }
        public string RedirectUri { get; set; }
        public Guid? AccessTokenId { get; set; }
        public Guid ClientId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        public virtual AccessToken AccessToken { get; set; }

        public bool IsExpired => ExpirationDate < DateTime.UtcNow;
    }
}
