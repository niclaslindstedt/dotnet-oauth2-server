using System;

namespace Etimo.Id.Entities
{
    public class JwtToken
    {
        public Guid TokenId { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string RefreshToken { get; set; }
        public string Scope { get; set; }

        public AccessToken ToAccessToken()
        {
            return new AccessToken
            {
                AccessTokenId = TokenId,
                ExpirationDate = ExpiresAt,
                Scope = Scope
            };
        }
    }
}
