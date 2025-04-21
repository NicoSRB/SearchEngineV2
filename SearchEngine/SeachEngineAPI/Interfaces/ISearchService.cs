using Microsoft.OpenApi.Services;
using Shared.Model;

namespace SeachEngineAPI.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResultQuery> SearchAsync(string[] query, bool caseSensitive, int maxAmount, bool includeTimeStamps);

        Task<List<int>> GetWordIdsAsync(string[] query, List<string> outIgnored);

        Task<List<int>> GetDocumentsAsync(List<int> wordIds);

        Task<List<BEDocument>> GetDocDetailsAsync(List<int> docIds);
    }

}
