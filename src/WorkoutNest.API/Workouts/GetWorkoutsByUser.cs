using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Workouts;

public class GetWorkoutsByUser: EndpointWithoutRequest<IEnumerable<GetWorkoutsByUser.GetWorkoutsByUserSimpleResponse>>
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
        
        var simpleWorkoutFeed = userWorkouts.Select(x => new GetWorkoutsByUserSimpleResponse()
        {
            Name = x.Name,
            WorkoutDate = x.Date,
            DaysPassedFromNow = (x.Date - DateTimeOffset.Now).Days,
            Value = userWorkouts
               .SelectMany(w => w.Exercises)
               .SelectMany(ew => ew.Sets)
               .Sum(set => set.Reps * set.Weight)
        });
        
        await SendAsync(simpleWorkoutFeed ,cancellation: c);

    }
    
    public class GetWorkoutsByUserSimpleResponse
    {
        public DateTimeOffset WorkoutDate { get; set; }
        public int DaysPassedFromNow { get; set; }
        public double Value { get; set; }
        public string Name { get; set; }
    }
}