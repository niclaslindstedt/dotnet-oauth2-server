namespace Etimo.Id.Entities
{
    public class ResourceOwnerPasswordCredentialsTokenRequest : TokenRequest
    {
        /// <summary>
        ///     Resource Owner Password Credentials Grant.
        ///     https://tools.ietf.org/html/rfc6749#section-4.3.2
        /// </summary>
        public ResourceOwnerPasswordCredentialsTokenRequest(
            string username,
            string password,
            string scope)
        {
            GrantType = "password";
            Username  = username;
            Password  = password;
            Scope     = scope;
        }
    }
}
