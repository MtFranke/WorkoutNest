using FastEndpoints;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Exercises;

internal class AddExerciseEndpoint: Endpoint<ExerciseRequest, ExerciseResponse>
{
    private readonly IMongoWrapper _mongoWrapper;

    public AddExerciseEndpoint(IMongoWrapper mongoWrapper)
    {
        _mongoWrapper = mongoWrapper;
    }
    
    public override void Configure()
    {
        Post("/exercises");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExerciseRequest r, CancellationToken c)
    {
        var exercises = _mongoWrapper.GetCollection<Exercise>(Collections.ExercisesCollection);
        var id = Guid.NewGuid().ToString();
        var exercise = new Exercise(id, r.Name, r.PrimaryMuscelGroup, r.OtherMuscelGroup,
            r.Equipment);
        await exercises.InsertOneAsync(exercise, c);
        await SendAsync(new ExerciseResponse() {Id =id} , cancellation: c);
    }

}

public class ExerciseRequest
{
    public string Name { get; set; }
    public string PrimaryMuscelGroup { get; set; }
    public string OtherMuscelGroup { get; set; }
    public string Equipment { get; set; }
}

public class ExerciseResponse
{
    public string Id { get; set; }
}
