using System.Collections.Generic;

namespace Etimo.Id.Api.Settings
{
    public class RateLimiterSettings
    {
        public List<RateLimiterRule> Rules { get; set; }
    }

    public class RateLimiterRule
    {
        public string Name { get; set; }
        public List<string> RateLimitExceptions { get; set; }
        public int RateLimitResponseMilliseconds { get; set; }
        public int SuccessfulToFailedRatio { get; set; }
        public int SoftRequestLimit { get; set; }
        public int HardRequestLimit { get; set; }
        public int CallWindowSeconds { get; set; }
        public int BanForMinutes { get; set; }
    }
}
