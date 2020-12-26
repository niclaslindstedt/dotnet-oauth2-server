using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Etimo.Id.Api.Security
{
    public class RateLimiterContext
    {
        public RateLimiterContext(string cacheString)
        {
            var rules = JsonSerializer.Deserialize<string[]>(cacheString);
            foreach (var ruleString in rules)
            {
                var rule = new RateLimiterRuleContext(ruleString);
                Rules.Add(rule);
            }
        }

        public RateLimiterContext()
        {
        }

        public override string ToString()
        {
            var result = JsonSerializer.Serialize(Rules.Select(r => r.ToString()));
            Console.WriteLine(result);
            return result;
        }

        public List<RateLimiterRuleContext> Rules { get; set; } = new List<RateLimiterRuleContext>();
        public string IpNumber { get; set; }
        public int ResponseTime { get; set; }
    }
}
