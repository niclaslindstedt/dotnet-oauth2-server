namespace Etimo.Id.Settings
{
    public class JwtSettings
    {
        public string Issuer     { get; set; }
        public string Secret     { get; set; }
        public string PublicKey  { get; set; }
        public string PrivateKey { get; set; }
    }
}
