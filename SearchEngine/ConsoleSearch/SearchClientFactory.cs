using ConsoleSearch.Client;
using ConsoleSearch.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSearch
{
    public static class SearchClientFactory
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public static ISearchClient Create()
        {
            return new SearchClient(_httpClient);
        }
    }
}
