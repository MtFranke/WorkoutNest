using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Workouts;

public class GetWorkoutEndpoint: EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/workout/{workoutId}");
        AllowAnonymous();
    }
    
    private string mongoDbConnectionString;

    public GetWorkoutEndpoint(IConfiguration configuration)
    {
        mongoDbConnectionString = configuration["MongoDbConnectionString"];
    }
    
    public override async Task HandleAsync(CancellationToken c)
    {
        var workoutId = Route<string>("workoutId");
        var client = new MongoClient(mongoDbConnectionString);
        var db = client.GetDatabase("workoutnest");
        var workout = db.GetCollection<Workout>(Collections.WorkoutsCollection);
        var res = await workout.FindAsync(x => x.Id == workoutId, cancellationToken: c);
        var r = await res.SingleOrDefaultAsync();
        await SendAsync(r);
    }
}