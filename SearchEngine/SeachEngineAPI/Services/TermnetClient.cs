using SeachEngineAPI.Interfaces;
using Shared.Model;
namespace SeachEngineAPI.Services
{
    public class TermnetClient : ITermnetClient
    {
        private readonly HttpClient _httpClient;

        public TermnetClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Dictionary<string, List<string>>> ExpandQueryAsync(string query, string[] domains)
        {
            if (domains == null || domains.Length == 0)
            {
                // No domains selected, return just the original query with no synonyms
                return new Dictionary<string, List<string>>
        {
            { query, new List<string>() }
        };
            }

            var requestBody = new
            {
                Query = query,
                Domains = domains
            };

            var response = await _httpClient.PostAsJsonAsync("api/termnet/expand", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var termnetResponse = await response.Content.ReadFromJsonAsync<TermnetResponse>();

                if (termnetResponse?.ExpandedTerms == null)
                {
                    throw new Exception("Response contains null ExpandedTerms.");
                }

                var convertedTerms = termnetResponse.ExpandedTerms.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Select(synonym => synonym.Term).ToList()
                );

                return convertedTerms;
            }

            throw new Exception("Failed to expand query.");
        }

    }
}
