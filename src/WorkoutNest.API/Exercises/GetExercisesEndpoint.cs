using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Exercises;

internal class GetExercisesEndpoint:EndpointWithoutRequest
{
    private readonly IMongoWrapper _mongoWrapper;

    public GetExercisesEndpoint(IMongoWrapper mongoWrapper)
    {
        _mongoWrapper = mongoWrapper;
    }
    
    public override void Configure()
    {
        Get("/exercises");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var exercisesCollection = _mongoWrapper.GetCollection<Exercise>(Collections.ExercisesCollection);
        var exercises = await exercisesCollection.Find(_ => true).ToListAsync(cancellationToken: c);
        await SendAsync(exercises , cancellation: c);
    }
}
