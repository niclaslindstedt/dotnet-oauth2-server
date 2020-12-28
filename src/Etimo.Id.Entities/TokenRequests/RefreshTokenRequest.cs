namespace Etimo.Id.Entities
{
    public class RefreshTokenRequest : TokenRequest
    {
        /// <summary>
        ///     Refresh Token Grant.
        ///     https://tools.ietf.org/html/rfc6749#section-6
        /// </summary>
        public RefreshTokenRequest(
            string grantType,
            string refreshToken,
            string scope)
        {
            GrantType    = grantType;
            RefreshToken = refreshToken;
            Scope        = scope;
        }
    }
}
