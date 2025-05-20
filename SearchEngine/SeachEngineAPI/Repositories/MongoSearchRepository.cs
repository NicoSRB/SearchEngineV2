using MongoDB.Driver;
using SeachEngineAPI.Interfaces;
using Shared.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeachEngineAPI.Repositories
{
    public class MongoSearchRepository : ISearchRepository
    {
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<BEDocument> _documents;
        private readonly IMongoCollection<Word> _words;
        private readonly IMongoCollection<Occ> _occurrences;

        public MongoSearchRepository(IMongoDbContext context)
        {
            _context = context;
            _documents = context.doc;
            _words = context.word;
            _occurrences = context.Occ;
        }

        public async Task<List<int>> GetWordIdsAsync(string[] query, List<string> outIgnored)
        {
            var allWords = await _words.Find(_ => true).ToListAsync();
            var wordDict = allWords.ToDictionary(w => w.Name, w => w.Id);

            List<int> result = new();
            foreach (var word in query)
            {
                if (wordDict.TryGetValue(word, out int wordId))
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
            var filter = Builders<Occ>.Filter.In(o => o.WordId, wordIds);
            var docs = await _occurrences.Find(filter)
                .Project(o => o.DocumentId)
                .ToListAsync();

            return docs.Distinct().ToList();
        }

        public async Task<List<BEDocument>> GetDocDetailsAsync(List<int> docIds)
        {
            var filter = Builders<BEDocument>.Filter.In(d => d.mId, docIds);
            return await _documents.Find(filter).ToListAsync();
        }

        public async Task<List<string>> GetMissingWordsAsync(int docId, List<int> wordIds)
        {
            var filter = Builders<Occ>.Filter.And(
                Builders<Occ>.Filter.Eq(o => o.DocumentId, docId),
                Builders<Occ>.Filter.In(o => o.WordId, wordIds)
            );

            var presentWordIds = await _occurrences.Find(filter)
                .Project(o => o.WordId)
                .ToListAsync();

            var missingWordIds = wordIds.Except(presentWordIds).ToList();

            var missingWordsFilter = Builders<Word>.Filter.In(w => w.Id, missingWordIds);
            var missingWords = await _words.Find(missingWordsFilter)
                .Project(w => w.Name)
                .ToListAsync();

            return missingWords;
        }
    }
}
