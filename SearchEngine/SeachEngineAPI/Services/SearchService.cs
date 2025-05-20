using SeachEngineAPI.Interfaces;
using Shared.Model;

public class SearchService : ISearchService
{
    private readonly ISearchRepository _repo;

    public SearchService(ISearchRepository repo)
    {
        _repo = repo;
    }

    public async Task<SearchResultQuery> SearchAsync(string[] query, bool caseSensitive, int maxAmount, bool includeTimeStamp)
    {
        List<string> ignored = new();
        DateTime start = DateTime.Now;

        var wordIds = await _repo.GetWordIdsAsync(query, ignored);
        var docIds = await _repo.GetDocumentsAsync(wordIds);
        var top = docIds.Take(maxAmount).ToList();

        List<DocumentHit> docResults = new();
        int idx = 0;

        foreach (var doc in await _repo.GetDocDetailsAsync(top))
        {
            var missing = await _repo.GetMissingWordsAsync(doc.mId, wordIds);
            docResults.Add(new DocumentHit(doc, docIds[idx++], missing, includeTimeStamp));
        }

        return new SearchResultQuery(query, docIds.Count, docResults, ignored, DateTime.Now - start);
    }
}
