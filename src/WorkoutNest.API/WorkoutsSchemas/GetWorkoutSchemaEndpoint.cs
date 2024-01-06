using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.WorkoutsSchemas;

public class GetWorkoutSchemaEndpoint : EndpointWithoutRequest
{
    private readonly string _mongoDbConnectionString;
    private readonly string _mongoDb;
    public GetWorkoutSchemaEndpoint(IConfiguration configuration)
    {
        _mongoDbConnectionString = configuration["MongoDbConnectionString"];
        _mongoDb = configuration["MongoDb"];
    }
    
    public override void Configure()
    {
        Get("/workouts-schema/{workoutId}");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var workoutId = Route<string>("workoutId");
        var userId  = User.Claims.Single(x => x.Type == "user_id").Value;
        var client = new MongoClient(_mongoDbConnectionString);
        var db = client.GetDatabase(_mongoDb);
        var workoutsCollection = db.GetCollection<WorkoutSchema>(Collections.WorkoutsSchemaCollection);
        var userWorkouts = workoutsCollection
            .Find(x => x.UserID == userId && x.Id == workoutId)
            .SingleOrDefault(cancellationToken: ct);

        await SendAsync(userWorkouts, cancellation: ct);
    }
    
}
