using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.WorkoutsSchemas;

public class GetWorkoutsSchemaEndpoint : EndpointWithoutRequest
{
    private readonly IMongoWrapper _mongoWrapper;

    public GetWorkoutsSchemaEndpoint(IMongoWrapper mongoWrapper)
    {
        _mongoWrapper = mongoWrapper;
    }
    
    public override void Configure()
    {
        Get("/workouts-schema");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId  = User.Claims.Single(x => x.Type == "user_id").Value;
        var workoutsCollection = _mongoWrapper.GetCollection<WorkoutSchema>(Collections.WorkoutsSchemaCollection);
        var userWorkouts = await workoutsCollection.Find(x => x.UserID == userId).ToListAsync(cancellationToken: ct);

        await SendAsync(userWorkouts, cancellation: ct);
    }
    
}
