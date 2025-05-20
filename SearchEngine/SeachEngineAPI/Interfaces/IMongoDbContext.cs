using MongoDB.Driver;
using Shared.Model;

namespace SeachEngineAPI.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoCollection<Word> word { get; }
        IMongoCollection<BEDocument> doc { get; }
        IMongoCollection<Occ> Occ { get; }
    }
}
