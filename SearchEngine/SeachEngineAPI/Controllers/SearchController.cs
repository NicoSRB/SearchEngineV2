using Microsoft.AspNetCore.Mvc;
using SeachEngineAPI.Interfaces;
using SeachEngineAPI.Factories;
using SeachEngineAPI.Context;

namespace SeachEngineAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;


        public SearchController(SearchDb1Context dbContext)
        {
            _searchService = SearchServiceFactory.Create(dbContext);
        }

        [HttpGet("{query}")]
        public async Task<IActionResult> Search(string query, [FromQuery] int maxAmount, [FromQuery] bool caseSensitive = true, [FromQuery] bool includeTimeStamps = true)
        {   
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be empty.");
            }
            
            // Only convert to lowercase if case-sensitive mode is OFF
            query = caseSensitive ? query : query.ToLower();

            string[] queryArray = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            // Apply maxWords limit if provided
            if (maxAmount > 0)
            {
                queryArray = queryArray.Take(maxAmount).ToArray();
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
