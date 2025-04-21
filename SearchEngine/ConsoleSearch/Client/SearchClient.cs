using ConsoleSearch.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleSearch.Client
{
    public class SearchClient : ISearchClient
    {
        private readonly HttpClient _searchClient;

        public SearchClient(HttpClient searchClient)
        {
            _searchClient = searchClient;
        }

        public async Task<List<string>> SearchAsync(string query, int maxWords)
        {
            string url = $"https://localhost:7122/api/Search/{Uri.EscapeDataString(query)}/10?maxWords={maxWords}";

            HttpResponseMessage response = await _searchClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();

            Console.WriteLine(jsonResponse);

            return JsonConvert.DeserializeObject<List<string>>(jsonResponse);
        }

    }
}
