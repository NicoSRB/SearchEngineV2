using System.Diagnostics.Metrics;

namespace Shared
{
    public class SearchMetrics
    {
        public Counter<long> CacheHits { get; }
        public Counter<long> CacheMisses { get; }
        public Histogram<double> QueryDuration { get; }
        public Counter<long> BackendUsed { get; }

        public SearchMetrics(IMeterFactory meterFactory)
        {
            var meter = meterFactory.Create("SearchEngineAPI");

            CacheHits = meter.CreateCounter<long>("search_cache_hits", "hits", "Number of cache hits");
            CacheMisses = meter.CreateCounter<long>("search_cache_misses", "misses", "Number of cache misses");
            QueryDuration = meter.CreateHistogram<double>("search_query_duration_ms", "ms", "Search query duration in ms");
            BackendUsed = meter.CreateCounter<long>("search_backend_used", "calls", "Backend call count");
        }
    }
}
