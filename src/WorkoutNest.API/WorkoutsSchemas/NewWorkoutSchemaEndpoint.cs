using System.Security.Claims;
using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Workouts;

public class NewWorkoutSchemaEndpoint: Endpoint<CreateWorkoutRequest>
{
    private readonly string _mongoDbConnectionString;
    private readonly string _mongoDb;
    public NewWorkoutSchemaEndpoint(IConfiguration configuration)
    {
        _mongoDbConnectionString = configuration["MongoDbConnectionString"];
        _mongoDb = configuration["MongoDb"];
    }
    
    public override void Configure()
    {
        Post("/workouts-schema");
    }

    public override async Task HandleAsync(CreateWorkoutRequest r, CancellationToken c)
    {
         var userId  = User.Claims.Single(x => x.Type == "user_id").Value;
         var workoutId = Guid.NewGuid().ToString();
         var client = new MongoClient(_mongoDbConnectionString);
         var db = client.GetDatabase(_mongoDb);
         var workoutSchemaCollection = db.GetCollection<WorkoutSchema>(Collections.WorkoutsSchemaCollection);
         var workoutSchema = new WorkoutSchema()
         {
             Name = r.Name,
             UserID = userId,
             Id = workoutId,
             ExercisesId = r.Exercises
         };
         await workoutSchemaCollection.InsertOneAsync(workoutSchema, cancellationToken: c);
         await SendAsync(workoutId, cancellation: c);

    }
}

public class CreateWorkoutRequest
{
    public string Name { get; set; }
    public string[] Exercises { get; set; }
    public string UserId { get; set; }
}