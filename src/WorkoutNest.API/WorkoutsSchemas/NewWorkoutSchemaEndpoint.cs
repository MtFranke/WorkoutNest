using FastEndpoints;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Workouts;

public class NewWorkoutSchemaEndpoint: Endpoint<CreateWorkoutRequest>
{
    private readonly IMongoWrapper _mongoWrapper;
    
    public NewWorkoutSchemaEndpoint(IMongoWrapper mongoWrapper)
    {
        _mongoWrapper = mongoWrapper;
    }
    
    public override void Configure()
    {
        Post("/workouts-schema");
    }

    public override async Task HandleAsync(CreateWorkoutRequest r, CancellationToken c)
    {
         var userId  = User.Claims.Single(x => x.Type == "user_id").Value;
         var workoutId = Guid.NewGuid().ToString();
         var workoutSchemaCollection = _mongoWrapper.GetCollection<WorkoutSchema>(Collections.WorkoutsSchemaCollection);
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