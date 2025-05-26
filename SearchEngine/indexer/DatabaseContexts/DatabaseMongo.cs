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
            var user = Environment.GetEnvironmentVariable("MONGO_USER") ?? "root";
            var password = Environment.GetEnvironmentVariable("MONGO_PASSWORD") ?? "password";
            var host = Environment.GetEnvironmentVariable("MONGO_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("MONGO_PORT") ?? "27017";

            var mongoConnectionString = $"mongodb://{user}:{password}@{host}:{port}";
            var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);
            var client = new MongoClient(mongoClientSettings);
            _database = client.GetDatabase("indexerdb");    

            Console.WriteLine("MongoDB connected");
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
            var docs = wordIds.Select(wordId => new BsonDocument
            {
                { "docId", docId },
                { "wordId", wordId }
            });
            collection.InsertMany(docs);
        }

        public Dictionary<string, int> GetAllWords()
        {
            var collection = _database.GetCollection<BsonDocument>("words");
            var words = collection.Find(FilterDefinition<BsonDocument>.Empty).ToList();
            var dict = new Dictionary<string, int>();

            foreach (var wordDoc in words)
            {
                if (wordDoc.TryGetValue("name", out var nameBson) && wordDoc.TryGetValue("id", out var idBson))
                {
                    dict[nameBson.AsString] = idBson.AsInt32;
                }
            }
            return dict;
        }

        public int GetDocumentCounts()
        {
            var collection = _database.GetCollection<BEDocument>("documents");
            return (int)collection.CountDocuments(FilterDefinition<BEDocument>.Empty);
        }
    }
}
