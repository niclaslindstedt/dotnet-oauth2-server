namespace Etimo.Id.Settings
{
    public class OAuthSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Authority { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
        public int TokenExpirationMinutes { get; set; }
    }
}
