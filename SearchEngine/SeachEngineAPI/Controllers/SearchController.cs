using Microsoft.AspNetCore.Mvc;
using SeachEngineAPI.Interfaces;
using System.Text.Json;
using Shared;
using System.Diagnostics;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;
using Shared.Model;

namespace SeachEngineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly ITermnetClient _termnetClient;
        private readonly ICacheService _cacheService;

        // Metrics
        private readonly SearchMetrics _metrics;
        private readonly Stopwatch _timer = new ();

        private readonly Counter<int> _cacheHits;
        private readonly Counter<int> _cacheMisses;

        public SearchController(ISearchService searchService,ITermnetClient termnetClient, ICacheService cacheService, SearchMetrics metrics, Meter meter)
        {
            _searchService = searchService;
            _termnetClient = termnetClient;
            _metrics = metrics;
            _cacheService = cacheService; 

            _cacheHits = meter.CreateCounter<int>("Search_Cache_Hits");
            _cacheMisses = meter.CreateCounter<int>("Search_Cache_Misses");
        }



        [HttpGet("")]
        public async Task<IActionResult> Search(
            [FromQuery] string query,
            [FromQuery] int maxAmount = 10,
            [FromQuery] bool caseSensitive = false,
            [FromQuery] bool includeTimeStamps = true,
            [FromQuery] string[]? domains = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be empty.");
            }

            _timer.Restart();

            string cacheKey = $"search:{query.ToLower()}:{string.Join(",", domains ?? [])}:{caseSensitive}:{maxAmount}";

            var cachedJson = await _cacheService.GetCachedResultAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedJson))
            {
                Console.WriteLine("Cache hit: " + cacheKey);
                _cacheHits.Add(1);
                _metrics.CacheHits.Add(1);
                _timer.Stop();
                _metrics.QueryDuration.Record(_timer.Elapsed.TotalMilliseconds);

                var cachedResponse = JsonSerializer.Deserialize<SearchResultQuery>(cachedJson);
                return Ok(cachedResponse);
            }

            _metrics.CacheMisses.Add(1);
            Console.WriteLine("Cache miss for " + cacheKey);
            _cacheMisses.Add(1);

            string[] queryArray;

            if (domains != null && domains.Length > 0)
            {
                var expandedTermsMap = await _termnetClient.ExpandQueryAsync(query, domains);

                queryArray = expandedTermsMap
                    .SelectMany(kvp => kvp.Value.Append(kvp.Key))
                    .Distinct()
                    .ToArray();
            }
            else
            {
                queryArray = new[] { query }; // no expansion, just use original
            }

            var results = await _searchService.SearchAsync(queryArray, caseSensitive, maxAmount, includeTimeStamps);

            if (results == null || results.DocumentHits.Count == 0)
            {
                return NotFound("No matches found.");
            }

            _timer.Stop();
            _metrics.QueryDuration.Record(_timer.Elapsed.TotalMilliseconds);


            var response = new SearchResultQuery
            {
                Query = queryArray,
                Hits = results.Hits,
                DocumentHits = results.DocumentHits,
                Ignored = results.Ignored,
                TimeUsed = results.TimeUsed,
                DbType = results.DbType
            };

            var backend = response.DbType;
            _metrics.BackendUsed.Add(1, new KeyValuePair<string, object?>("backend", backend));


            await _cacheService.SetCachedResultAsync(cacheKey, JsonSerializer.Serialize(response), TimeSpan.FromMinutes(10));

            return Ok(response);
        }
    }
}
