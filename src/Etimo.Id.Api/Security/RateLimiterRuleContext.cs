using Etimo.Id.Api.Settings;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Etimo.Id.Api.Security
{
    public class RateLimiterRuleContext
    {
        public RateLimiterRuleContext(string ruleString)
        {
            string[] values = JsonSerializer.Deserialize<string[]>(ruleString);
            BannedUntil      = values[0] != "0" ? DateTime.Parse(values[0]) : null;
            WindowExpiration = DateTime.Parse(values[1]);
            SoftRequests     = int.Parse(values[2]);
            HardRequests     = int.Parse(values[3]);
            HarmlessRequests = int.Parse(values[4]);
        }

        public RateLimiterRuleContext(RateLimiterRule rule)
        {
            Rule             = rule;
            WindowExpiration = DateTime.UtcNow.AddSeconds(rule.CallWindowSeconds);
        }

        public string             Name             { get => Rule?.Name; }
        public bool               Banned           { get => BannedUntil != null ? DateTime.UtcNow < BannedUntil : false; }
        public DateTime?          BannedUntil      { get; set; }
        public DateTime           WindowExpiration { get; set; }
        public int                SoftRequests     { get; set; }
        public int                HardRequests     { get; set; }
        public int                HarmlessRequests { get; set; }
        public bool               ExistsInCache    { get; set; }
        public RateLimiterContext Context          { get; set; }
        public RateLimiterRule    Rule             { get; set; }

        public int SecondsLeftOnBan { get => Banned ? (int)(BannedUntil.Value - DateTime.UtcNow).TotalSeconds : 0; }

        public override string ToString()
        {
            var values = new List<string>();
            values.Add(BannedUntil != null ? BannedUntil.ToString() : "0");
            values.Add(WindowExpiration.ToString());
            values.Add(SoftRequests.ToString());
            values.Add(HardRequests.ToString());
            values.Add(HarmlessRequests.ToString());

            return JsonSerializer.Serialize(values);
        }
    }
}
