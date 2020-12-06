// ReSharper disable InconsistentNaming

using Etimo.Id.Entities;

namespace Etimo.Id.Api.OAuth
{
    public class AccessTokenResponseDto
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }

        public static AccessTokenResponseDto FromJwtToken(JwtToken token)
        {
            return new AccessTokenResponseDto
            {
                access_token = token.AccessToken,
                token_type = token.TokenType,
                expires_in = token.ExpiresIn,
                refresh_token = token.RefreshToken,
                scope = token.Scope
            };
        }
    }
}
