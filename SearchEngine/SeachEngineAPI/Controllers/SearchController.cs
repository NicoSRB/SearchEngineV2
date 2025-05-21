using Microsoft.AspNetCore.Mvc;
using SeachEngineAPI.Interfaces;
using System.Text.Json;
using Shared;
using System.Diagnostics;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

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

        // private readonly Counter<int> _cacheHits;
        // private readonly Counter<int> _cacheMisses;

        public SearchController(ISearchService searchService,ITermnetClient termnetClient, SearchMetrics metrics, /*ICacheService cacheService,*/ IMeterFactory meterFactory)
        {
            _searchService = searchService;
            _termnetClient = termnetClient;
            _metrics = metrics;

            //_cacheService = cacheService;

            var meter = meterFactory.Create("SearchEngineAPI");

            //_cacheHits = meter.CreateCounter<int>("Search_Cache_Hits");
            //_cacheMisses = meter.CreateCounter<int>("Search_Cache_Misses");
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

            //string? cached = await _cacheService.GetCachedResultAsync(cacheKey);
            bool cacheHit = false;
            //bool cacheHit = TryGetFromCache(query, out var result);
            //if (cacheHit)m
            //{
            //    _metrics.CacheHits.Add(1);
            //    _timer.Stop();
            //    _metrics.QueryDuration.Record(_timer.Elapsed.TotalMilliseconds);
            //    return Ok(result);
            //}

            _metrics.CacheMisses.Add(1);

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


            var response = new
            {
                query = queryArray,
                hits = results.Hits,
                documentHits = results.DocumentHits,
                ignored = results.Ignored,
                timeUsed = results.TimeUsed,
                dbType = results.DbType,
            };

            var backend = response.dbType;
            _metrics.BackendUsed.Add(1, new KeyValuePair<string, object?>("backend", backend));

            //await _cacheService.SetCachedResultAsync(cacheKey, JsonSerializer.Serialize(response), TimeSpan.FromMinutes(10));

            return Ok(response);
        }
    }
}
