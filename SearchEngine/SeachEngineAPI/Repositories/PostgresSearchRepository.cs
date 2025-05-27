using SeachEngineAPI.DbContexts;
using SeachEngineAPI.Interfaces;
using Shared.Model;
using Microsoft.EntityFrameworkCore;

public class PostgresSearchRepository : ISearchRepository
{
    private readonly postgreDbContext _context;

    public PostgresSearchRepository(postgreDbContext context)
    {
        _context = context;
    }

    public async Task<List<int>> GetWordIdsAsync(string[] query, List<string> outIgnored)
    {
        var words = await _context.word.ToDictionaryAsync(w => w.Name, w => w.Id);
        List<int> result = new();

        foreach (var word in query)
        {
            if (words.TryGetValue(word, out int wordId))
                result.Add(wordId);
            else
                outIgnored.Add(word);
        }   

        return result;
    }

    public async Task<List<int>> GetDocumentsAsync(List<int> wordIds)
    {
        return await _context.occ
            .Where(o => wordIds.Contains(o.WordId))
            .Select(o => o.DocumentId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<BEDocument>> GetDocDetailsAsync(List<int> docIds)
    {
        return await _context.document
            .Where(d => docIds.Contains(d.mId))
            .Select(d => new BEDocument
            {
                mId = d.mId,
                mUrl = d.mUrl,
                mCreationTime = d.mCreationTime,
                mIdxTime = d.mIdxTime,
            })
            .ToListAsync();
    }

    public async Task<List<string>> GetMissingWordsAsync(int docId, List<int> wordIds)
    {
        var presentWordIds = await _context.occ
            .Where(o => o.DocumentId == docId && wordIds.Contains(o.WordId))
            .Select(o => o.WordId)
            .ToListAsync();

        var missingWordIds = wordIds.Except(presentWordIds).ToList();

        return await _context.word
            .Where(w => missingWordIds.Contains(w.Id))
            .Select(w => w.Name)
            .ToListAsync();
    }
}
