namespace WorkoutNest.Infrastructure.Mongo.Entities;

public class WorkoutSchema
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string UserID { get; set; }
    public string[] ExercisesId { get; set; }
}