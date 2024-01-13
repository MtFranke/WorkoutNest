using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace WorkoutNest.Infrastructure.Mongo;

public class MongoWrapper : IMongoWrapper
{
    private string mongoDbConnectionString;
    private string _mongoDb = "workoutnest";

    public MongoWrapper(IConfiguration configuration)
    {
        mongoDbConnectionString = configuration["MongoDbConnectionString"];
    }
    
    public IMongoCollection<TCollection> GetCollection<TCollection>(string collection)
    {
        var client = new MongoClient(mongoDbConnectionString);
        var db = client.GetDatabase(_mongoDb);
        var workoutsCollection = db.GetCollection<TCollection>(collection);
        return workoutsCollection;
    }
}

public interface IMongoWrapper
{
    IMongoCollection<TCollection> GetCollection<TCollection>(string collection);
}