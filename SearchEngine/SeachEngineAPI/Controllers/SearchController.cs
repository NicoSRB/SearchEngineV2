using Microsoft.AspNetCore.Mvc;
using SeachEngineAPI.Interfaces;
using System.Text.Json;

namespace SeachEngineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly ITermnetClient _termnetClient;
        private readonly ICacheService _cacheService;

       // private readonly Counter<int> _cacheHits;
       // private readonly Counter<int> _cacheMisses;

        public SearchController(ISearchService searchService,ITermnetClient termnetClient /*ICacheService cacheService, IMeterFactory meterFactory*/)
        {
            _searchService = searchService;
            _termnetClient = termnetClient;
            //_cacheService = cacheService;

            //var meter = meterFactory.Create("SearchEngineAPI");

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

            string cacheKey = $"search:{query.ToLower()}:{string.Join(",", domains ?? [])}:{caseSensitive}:{maxAmount}";
            //string? cached = await _cacheService.GetCachedResultAsync(cacheKey);

            //if (cached != null)
            //{
            //    _cacheHits.Add(1);
            //    return Content(cached, "application/json");
            //}

            //_cacheMisses.Add(1);

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


            var response = new
            {
                query = queryArray,
                hits = results.Hits,
                documentHits = results.DocumentHits,
                ignored = results.Ignored,
                timeUsed = results.TimeUsed,
                dbType = results.DbType,
            };

            //await _cacheService.SetCachedResultAsync(cacheKey, JsonSerializer.Serialize(response), TimeSpan.FromMinutes(10));

            return Ok(response);
        }
    }
}
