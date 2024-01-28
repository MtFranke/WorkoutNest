using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Workouts;

public class GetWorkoutsByUser: EndpointWithoutRequest<List<Workout>>
{
    private readonly IMongoWrapper _mongoWrapper;

    public override void Configure()
    {
        Get("/workouts");
    }
    

    public GetWorkoutsByUser(IMongoWrapper mongoWrapper)
    {
        _mongoWrapper = mongoWrapper;
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var userId  = User.Claims.Single(x => x.Type == "user_id").Value;
        var workouts =  _mongoWrapper.GetCollection<Workout>(Collections.WorkoutsCollection);
        var userWorkouts = await (await workouts.FindAsync(workout => workout.UserId == userId, cancellationToken: c))
            .ToListAsync(c);
        
        await SendAsync(userWorkouts ,cancellation: c);

    }
}