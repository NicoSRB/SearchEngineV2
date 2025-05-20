using indexer.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace indexer.Databases
{
    public class DatabaseMongo : IDatabase
    {
        private readonly IMongoDatabase _database;
        public DatabaseMongo()
        {
            var mongoClientSettings = MongoClientSettings.FromConnectionString("mongodb://localhost:27017");
            var client = new MongoClient(mongoClientSettings);

            var database = client.GetDatabase("db");
        }

        public void InsertDocument(BEDocument doc)
        {
            var collection = _database.GetCollection<BEDocument>("documents");
            collection.InsertOne(doc);
        }

        public void InsertWord(int id, string value)
        {
            var collection = _database.GetCollection<BsonDocument>("words");
            var doc = new BsonDocument
                {
                    { "id", id },
                    { "name", value }
                };
            collection.InsertOne(doc);
        }

        public void InsertAllWords(Dictionary<string, int> res)
        {
            var collection = _database.GetCollection<BsonDocument>("words");
            var docs = res.Select(kv => new BsonDocument { { "id", kv.Value }, { "name", kv.Key } });
            collection.InsertMany(docs);
        }
        public void InsertAllOcc(int docId, ISet<int> wordIds)
        {
            var collection = _database.GetCollection<BsonDocument>("occurrences");
            var docs = wordIds.Select(wordId => new BsonDocument {
            { "docId", docId }, { "wordId", wordId }
        });
            collection.InsertMany(docs);
        }

        public Dictionary<string, int> GetAllWords() => throw new NotImplementedException();
        public int GetDocumentCounts() => throw new NotImplementedException();
    }
}
