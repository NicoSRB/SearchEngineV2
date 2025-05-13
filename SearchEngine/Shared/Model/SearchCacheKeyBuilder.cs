using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public static class SearchCacheKeyBuilder
    {
        public static string BuildCacheKey(string[] query, bool caseSensitive, int maxAmount, List<string> domains, bool includeTimeStamps)
        {
            var domainPart = domains != null ? string.Join(",", domains) : "";
            return $"search:{query}:{caseSensitive}:{maxAmount}:{includeTimeStamps}:{domainPart}";
        }
    }
}
