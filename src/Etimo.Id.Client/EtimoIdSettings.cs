namespace Etimo.Id.Client
{
    public class EtimoIdSettings
    {
        public string ClientId     { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri  { get; set; }
        public string Issuer       { get; set; }
        public string Audience     { get; set; }
        public string Secret       { get; set; }
    }
}
