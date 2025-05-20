using Shared.Model;

namespace SeachEngineAPI.Interfaces
{
    public interface ISearchRepository
    {
        Task<List<int>> GetWordIdsAsync(string[] query, List<string> outIgnored);
        Task<List<int>> GetDocumentsAsync(List<int> wordIds);
        Task<List<BEDocument>> GetDocDetailsAsync(List<int> docIds);
        Task<List<string>> GetMissingWordsAsync(int docId, List<int> wordIds);
    }
}
