using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Workouts;

public class GetWorkoutEndpoint: EndpointWithoutRequest
{
    private readonly IMongoWrapper _mongoWrapper;

    public override void Configure()
    {
        Get("/workout/{workoutId}");
    
    }
    

    public GetWorkoutEndpoint(IMongoWrapper mongoWrapper)
    {
        _mongoWrapper = mongoWrapper;
    }
    
    public override async Task HandleAsync(CancellationToken c)
    {
        var workoutId = Route<string>("workoutId");
        var workout =  _mongoWrapper.GetCollection<Workout>(Collections.WorkoutsCollection);
        var res = await workout.FindAsync(x => x.Id == workoutId, cancellationToken: c);
        var r = await res.SingleOrDefaultAsync();
        await SendAsync(r);
    }
}