using System.Collections.Generic;

namespace Etimo.Id.Settings
{
    public class SiteSettings
    {
        public string       ListenUri   { get; set; }
        public List<string> TlsVersions { get; set; }
    }
}
