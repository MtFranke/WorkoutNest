using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Exercises;

internal class GetExercisesEndpoint:EndpointWithoutRequest
{
    private string mongoDbConnectionString;

    public GetExercisesEndpoint(IConfiguration configuration)
    {
        mongoDbConnectionString = configuration["MongoDbConnectionString"];
    }
    
    public override void Configure()
    {
        Get("/exercises");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var client = new MongoClient(mongoDbConnectionString);
        var db = client.GetDatabase("workoutnest");
        var exercisesCollection = db.GetCollection<Exercise>(Collections.ExercisesCollection);
        var exercises = await exercisesCollection.Find(_ => true).ToListAsync(cancellationToken: c);

        await SendAsync(new ExercisesResponse() {Exercises = exercises} , cancellation: c);

    }
}

public class ExercisesResponse
{
    public IList<Exercise> Exercises { get; set; }
}