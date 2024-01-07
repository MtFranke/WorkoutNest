using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Workouts;

public class SaveWorkoutEndpoint : Endpoint<SaveWorkoutEndpoint.SaveWorkoutRequest, string>
{
    private string mongoDbConnectionString;

    public SaveWorkoutEndpoint(IConfiguration configuration)
    {
        mongoDbConnectionString = configuration["MongoDbConnectionString"];
    }
    
    public override void Configure()
    {
        Post("/workout");
    }
    
    public override async Task HandleAsync(SaveWorkoutRequest r, CancellationToken c)
    {
        var userId  = User.Claims.Single(x => x.Type == "user_id").Value;
        var client = new MongoClient(mongoDbConnectionString);
        var db = client.GetDatabase("workoutnest");
        var workout = db.GetCollection<Workout>(Collections.WorkoutsCollection);
        var workoutDone = new Workout()
        {
            Name = r.Name,
            Id = Guid.NewGuid().ToString(),
            Exercises = r.Exercises,
            UserId = userId,
            Date = DateTimeOffset.Now
        };
        await workout.InsertOneAsync(workoutDone, c);

        await SendAsync(workoutDone.Id);

    }
    
    public class SaveWorkoutRequest
    {
        public string Name { get; set; }
        public List<ExerciseWorkout> Exercises { get; set; }
    }
    
 
}