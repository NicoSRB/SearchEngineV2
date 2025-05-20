using Microsoft.OpenApi.Services;
using Shared.Model;

namespace SeachEngineAPI.Interfaces
{
    public interface ISearchService
    {
        Task<SearchResultQuery> SearchAsync(string[] query, bool caseSensitive, int maxAmount, bool includeTimeStamp);
    }

}
