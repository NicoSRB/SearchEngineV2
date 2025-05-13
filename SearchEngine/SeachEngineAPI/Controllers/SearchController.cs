using Microsoft.AspNetCore.Mvc;
using SeachEngineAPI.Interfaces;
using SeachEngineAPI.Factories;
using SeachEngineAPI.Context;
using Shared.Model;

namespace SeachEngineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly ITermnetClient _termnetClient;

        public SearchController(SearchDb1Context dbContext, ITermnetClient termnetClient)
        {
            _searchService = SearchServiceFactory.Create(dbContext);
            _termnetClient = termnetClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Search(
            [FromQuery] string query, 
            [FromQuery] int maxAmount = 10, 
            [FromQuery] bool caseSensitive = false, 
            [FromQuery] bool includeTimeStamps = true,
            [FromQuery] string[]? domains = null
            )
        {   
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be empty.");
            }


            string[] queryArray = QueryProcessor.ProcessQuery(query, caseSensitive, maxAmount, domains);

            if (domains != null && domains.Length > 0)
            {
                var expandedTermsMap = await _termnetClient.ExpandQueryAsync(query, domains);

                var expandedQuery = expandedTermsMap
                    .SelectMany(kvp => kvp.Value.Append(kvp.Key))
                    .Distinct()
                    .ToArray();

                queryArray = expandedQuery;
            }

            var results = await _searchService.SearchAsync(queryArray, caseSensitive, maxAmount, includeTimeStamps);


            if (results == null || results.DocumentHits.Count == 0)
            {
                return NotFound("No matches found.");
            }

            return Ok(new
            {
                query = queryArray,  // Expanded query
                hits = results.Hits,
                documentHits = results.DocumentHits,
                ignored = results.Ignored,
                timeUsed = results.TimeUsed
            });
        }
    }
}
