namespace Etimo.Id.Entities
{
    public class RefreshTokenRequest : TokenRequest
    {
        /// <summary>
        ///     Refresh Token Grant.
        ///     https://tools.ietf.org/html/rfc6749#section-6
        /// </summary>
        public RefreshTokenRequest(string refreshToken, string scope)
        {
            GrantType    = "refresh_token";
            RefreshToken = refreshToken;
            Scope        = scope;
        }
    }
}
