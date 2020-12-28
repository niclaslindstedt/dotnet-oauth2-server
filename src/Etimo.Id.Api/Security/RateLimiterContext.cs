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
            string[]? rules = JsonSerializer.Deserialize<string[]>(cacheString);
            foreach (string ruleString in rules)
            {
                var rule = new RateLimiterRuleContext(ruleString);
                Rules.Add(rule);
            }
        }

        public RateLimiterContext() { }

        public List<RateLimiterRuleContext> Rules        { get; set; } = new();
        public string                       IpNumber     { get; set; }
        public int                          ResponseTime { get; set; }

        public override string ToString()
        {
            string result = JsonSerializer.Serialize(Rules.Select(r => r.ToString()));
            Console.WriteLine(result);
            return result;
        }
    }
}
