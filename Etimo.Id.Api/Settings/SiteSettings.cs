using System.Collections.Generic;

namespace Etimo.Id.Api.Settings
{
    public class SiteSettings
    {
        public string ListenUri { get; set; }
        public List<string> TlsVersions { get; set; }
    }
}
