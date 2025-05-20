using MongoDB.Driver;
using Shared.Model;
using SeachEngineAPI.Interfaces;

namespace SeachEngineAPI.DbContexts
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["Mongo:ConnectionString"]);
            _database = client.GetDatabase(configuration["Mongo:Database"]);
        }


        public IMongoCollection<BEDocument> doc => _database.GetCollection<BEDocument>("document");
        public IMongoCollection<Word> word => _database.GetCollection<Word>("word");
        public IMongoCollection<Occ> Occ => _database.GetCollection<Occ>("occ");


 

    }
}
