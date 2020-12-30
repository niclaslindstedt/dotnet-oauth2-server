namespace Etimo.Id.Entities
{
    public class ClientCredentialsTokenRequest : TokenRequest
    {
        /// <summary>
        ///     Client Credentials Grant.
        ///     https://tools.ietf.org/html/rfc6749#section-4.4.2
        /// </summary>
        public ClientCredentialsTokenRequest(string scope)
        {
            GrantType = "client_credentials";
            Scope     = scope;
        }
    }
}
