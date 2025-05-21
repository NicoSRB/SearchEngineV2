using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

namespace Shared
{
    public class SearchMetrics
    {
        private static readonly Meter Meter = new("SearchEngineMetrics", "1.0.0");

        public Counter<long> CacheHits { get; } =
            Meter.CreateCounter<long>("search_cache_hits", "hits", "Number of cache hits");

        public Counter<long> CacheMisses { get; } =
            Meter.CreateCounter<long>("search_cache_misses", "misses", "Number of cache misses");

        public Histogram<double> QueryDuration { get; } =
            Meter.CreateHistogram<double>("search_query_duration_ms", "ms", "Search query duration in ms");

        public Counter<long> BackendUsed { get; } =
            Meter.CreateCounter<long>("search_backend_used", "calls", "Backend call count");
    }
}
