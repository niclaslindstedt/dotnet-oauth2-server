namespace Etimo.Id.Service.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public int LifetimeMinutes { get; set; }
    }
}
