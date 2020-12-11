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
            var values = JsonSerializer.Deserialize<string[]>(ruleString);
            BannedUntil = values[0] != "0" ? DateTime.Parse(values[0]) : null;
            SoftRequests = int.Parse(values[1]);
            HardRequests = int.Parse(values[2]);
            HarmlessRequests = int.Parse(values[3]);
        }

        public RateLimiterRuleContext(RateLimiterRule rule)
        {
            Rule = rule;
        }

        public override string ToString()
        {
            var values = new List<string>();
            values.Add(BannedUntil != null ? BannedUntil.ToString() : "0");
            values.Add(SoftRequests.ToString());
            values.Add(HardRequests.ToString());
            values.Add(HarmlessRequests.ToString());

            return JsonSerializer.Serialize(values);
        }

        public string Name => Rule?.Name;
        public bool Banned => BannedUntil != null ? DateTime.UtcNow < BannedUntil : false;
        public DateTime? BannedUntil { get; set; }
        public int SoftRequests { get; set; }
        public int HardRequests { get; set; }
        public int HarmlessRequests { get; set; }
        public bool ExistsInCache { get; set; }
        public int CallWindowSeconds { get; set; }
        public RateLimiterContext Context { get; set; }
        public RateLimiterRule Rule { get; set; }

        public int SecondsLeftOnBan => Banned ? (int)((BannedUntil.Value - DateTime.UtcNow).TotalSeconds) : 0;
    }
}