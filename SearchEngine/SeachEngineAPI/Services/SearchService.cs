using Microsoft.EntityFrameworkCore;
using SeachEngineAPI.Context;
using SeachEngineAPI.Interfaces;
using Shared.Model;

namespace SeachEngineAPI.Services
{
    public class SearchService : ISearchService
    {
        private readonly SearchDb1Context _context;

        public SearchService(SearchDb1Context context)
        {
            _context = context;
        }

        public async Task<SearchResultQuery> SearchAsync(string[] query, bool caseSensitive, int maxAmount, bool includeTimeStamp)
        {
            List<string> ignored = new();
            DateTime start = DateTime.Now;

            // Convert words to word IDs
            var wordIds = await GetWordIdsAsync(query, ignored);

            // Perform the search - get all document IDs
            var docIds = await GetDocumentsAsync(wordIds);

            // Get the top maxAmount document IDs
            var top = docIds.Take(maxAmount).ToList();

            // Compose the result
            List<DocumentHit> docResults = new();
            int idx = 0;

            foreach (var doc in await GetDocDetailsAsync(top))
            {
                var missing = await GetMissingWordsAsync(doc.mId, wordIds);

                docResults.Add(new DocumentHit(doc, docIds[idx++], missing, includeTimeStamp));
            }

            return new SearchResultQuery(query, docIds.Count, docResults, ignored, DateTime.Now - start);
        }

        public async Task<List<int>> GetWordIdsAsync(string[] query, List<string> outIgnored)
        {
            var words = await _context.word.ToDictionaryAsync(w => w.Name, w => w.Id);

            List<int> result = new();
            foreach (var word in query)
            {
                if (words.TryGetValue(word, out int wordId))
                {
                    result.Add(wordId);
                }
                else
                {
                    outIgnored.Add(word);
                }
            }

            return result;
        }

        public async Task<List<int>> GetDocumentsAsync(List<int> wordIds)
        {
            var documentIds = await _context.Occ
                .Where(o => wordIds.Contains(o.WordId))
                .Select(o => o.DocumentId) // Select only the document IDs
                .Distinct() // Ensure unique document IDs
                .ToListAsync(); // Fetch data

            return documentIds;
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

        private async Task<List<string>> GetMissingWordsAsync(int docId, List<int> wordIds)
        {
            var presentWordIds = await _context.Occ
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



}
