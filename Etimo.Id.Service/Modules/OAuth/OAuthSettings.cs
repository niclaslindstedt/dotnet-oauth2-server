namespace Etimo.Id.Service.OAuth
{
    public class OAuthSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int TokenExpirationMinutes { get; set; }
    }
}
