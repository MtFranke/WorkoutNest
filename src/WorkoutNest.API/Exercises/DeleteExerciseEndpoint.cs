using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Exercises;

internal class DeleteExerciseEndpoint : EndpointWithoutRequest
{
    
    private string mongoDbConnectionString;

    public DeleteExerciseEndpoint(IConfiguration configuration)
    {
        mongoDbConnectionString = configuration["MongoDbConnectionString"];
    }
    
    public override void Configure()
    {
        Delete("/exercises/{exerciseId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var exerciseId = Route<string>("exerciseId");
        var client = new MongoClient(mongoDbConnectionString);
        var db = client.GetDatabase("workoutnest");
        var exercises = db.GetCollection<Exercise>(Collections.ExercisesCollection);

        await exercises.DeleteOneAsync(x => x.Id == exerciseId, cancellationToken: c);
    }

}