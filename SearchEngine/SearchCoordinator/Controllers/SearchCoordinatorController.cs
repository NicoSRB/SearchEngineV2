using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Shared.Model;
using Microsoft.AspNetCore.WebUtilities;

namespace SearchCoordinator.Controllers
{

    [Route("api/search")]
    [ApiController]
    public class SearchCoordinatorController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public SearchCoordinatorController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // List of backend Search API base URLs
        private List<string> DBcalls = new List<string>
        {
            "http://localhost:5267/",
            "http://localhost:5268/"
        };

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] string query,
            [FromQuery] int maxAmount = 10,
            [FromQuery] bool caseSensitive = false,
            [FromQuery] bool includeTimeStamps = true,
            [FromQuery] string[]? domains = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query cannot be empty.");

            var searchTasks = new List<Task<HttpResponseMessage>>();

            foreach (var dbBaseUrl in DBcalls)
            {
                var queryParams = new Dictionary<string, string?>
                {
                    { "query", query },
                    { "maxAmount", maxAmount.ToString() },
                    { "caseSensitive", caseSensitive.ToString().ToLower() },
                    { "includeTimeStamps", includeTimeStamps.ToString().ToLower() }
                };

                // Add domains as repeated query parameters
                if (domains != null && domains.Length > 0)
                {
                    foreach (var domain in domains)
                    {
                        queryParams.Add("domains", domain);
                    }
                }

                var fullUrl = QueryHelpers.AddQueryString($"{dbBaseUrl}api/search", queryParams);
                searchTasks.Add(_httpClient.GetAsync(fullUrl));
            }

            var responses = await Task.WhenAll(searchTasks);

            var combinedResults = new List<SearchResultQuery>();

            foreach (var response in responses)
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();

                    try
                    {
                        var result = JsonSerializer.Deserialize<SearchResultQuery>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (result != null)
                        {
                            Console.WriteLine($"Received results from DB: {result.DbType}");
                            combinedResults.Add(result);
                        }
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Failed to deserialize response: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch data from {response.RequestMessage?.RequestUri}");
                }
            }

            return Ok(combinedResults);
        }
    }
}
