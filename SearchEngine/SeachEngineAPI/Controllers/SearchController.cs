using Microsoft.AspNetCore.Mvc;
using SeachEngineAPI.Interfaces;
using SeachEngineAPI.Factories;
using SeachEngineAPI.Context;
using System.Runtime.InteropServices;
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

        [HttpGet("{query}")]
        public async Task<IActionResult> Search(
            string query, 
            [FromQuery] int maxAmount, 
            [FromQuery] bool caseSensitive = true, 
            [FromQuery] bool includeTimeStamps = true,
            [FromQuery] List<string>? domains = null,
            [FromQuery] bool expandedWithSynonym = false)
        {   
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be empty.");
            }

            string[] queryArray = QueryProcessor.ProcessQuery(query, caseSensitive, maxAmount);

            if (expandedWithSynonym)
            {
                // First step: 
                var expandedTermsMap = await _termnetClient.ExpandQueryAsync(query, domains ?? new List<string>());

                // Get the expanded terms
                var expandedTerms = expandedTermsMap
                    .SelectMany(kvp => kvp.Value.Append(kvp.Key))
                    .Distinct()
                    .ToArray();
            }

            var results = await _searchService.SearchAsync(queryArray, caseSensitive, maxAmount, includeTimeStamps);


            if (results == null || results.DocumentHits.Count == 0)
            {
                return NotFound("No matches found.");
            }

            return Ok(results);
        }
    }
}
