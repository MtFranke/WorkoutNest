namespace WorkoutNest.Infrastructure.Mongo.Entities;

public class Workout
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string UserId { get; set; }
    public DateTimeOffset Date { get; set; }
    
    public IList<ExerciseWorkout> Exercises { get; set; }
    
}

public class ExerciseWorkout
{
    public string ExercisesId { get; set; }
    public IList<Set> Sets { get; set; }
}

    
public class Set
{
    public int Reps { get; set; }
    public double Weight { get; set; }
}