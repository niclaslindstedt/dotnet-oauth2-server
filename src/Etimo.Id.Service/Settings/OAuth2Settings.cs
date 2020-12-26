namespace Etimo.Id.Service.Settings
{
    public class OAuth2Settings
    {
        public int AuthorizationCodeLifetimeMinutes { get; set; }
        public int AuthorizationCodeLength { get; set; }
        public int RefreshTokenLength { get; set; }
        public int RefreshTokenLifetimeDays { get; set; }
    }
}
