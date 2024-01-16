using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Exercises;

internal class DeleteExerciseEndpoint : EndpointWithoutRequest
{
    private readonly IMongoWrapper _mongoWrapper;

    private string mongoDbConnectionString;

    public DeleteExerciseEndpoint(IMongoWrapper mongoWrapper)
    {
        _mongoWrapper = mongoWrapper;
    }
    
    public override void Configure()
    {
        Delete("/exercises/{exerciseId}");
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var exerciseId = Route<string>("exerciseId");
        var exercises = _mongoWrapper.GetCollection<Exercise>(Collections.ExercisesCollection);
        await exercises.DeleteOneAsync(x => x.Id == exerciseId, cancellationToken: c);
    }

}