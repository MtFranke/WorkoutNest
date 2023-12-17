using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Exercises;

public class AddExerciseEndpoint: Endpoint<ExerciseRequest, ExerciseResponse>
{
    private string mongoDbConnectionString;

    public AddExerciseEndpoint(IConfiguration configuration)
    {
        mongoDbConnectionString = configuration["MongoDbConnectionString"];
    }
    
    public override void Configure()
    {
        Post("/exercises");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExerciseRequest r, CancellationToken c)
    {
        var client = new MongoClient(mongoDbConnectionString);
        var db = client.GetDatabase("workoutnest");
        var exercises = db.GetCollection<Exercise>(Collections.ExercisesCollection);
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
