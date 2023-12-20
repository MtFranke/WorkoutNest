using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Workouts;

public class GetWorkoutsSchemaEndpoint : EndpointWithoutRequest
{
    private readonly string _mongoDbConnectionString;
    private readonly string _mongoDb;
    public GetWorkoutsSchemaEndpoint(IConfiguration configuration)
    {
        _mongoDbConnectionString = configuration["MongoDbConnectionString"];
        _mongoDb = configuration["MongoDb"];
    }
    
    public override void Configure()
    {
        Get("/workouts-schema");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId  = User.Claims.Single(x => x.Type == "user_id").Value;
        var client = new MongoClient(_mongoDbConnectionString);
        var db = client.GetDatabase(_mongoDb);
        var workoutsCollection = db.GetCollection<WorkoutSchema>(Collections.WorkoutsSchemaCollection);
        var userWorkouts = await workoutsCollection.Find(x => x.UserID == userId).ToListAsync(cancellationToken: ct);

        await SendAsync(userWorkouts, cancellation: ct);
    }
    
}

public class GetWorkoutRequest
{
    public string Name { get; set; }
    public string[] Exercises { get; set; }
    public string UserId { get; set; }
}