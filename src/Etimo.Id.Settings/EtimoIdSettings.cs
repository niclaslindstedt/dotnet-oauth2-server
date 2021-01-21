namespace Etimo.Id.Settings
{
    public class EtimoIdSettings : JwtSettings
    {
        public string ServerUri    { get; set; }
        public string ClientId     { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri  { get; set; }
        public string Audience     { get; set; }
    }
}
