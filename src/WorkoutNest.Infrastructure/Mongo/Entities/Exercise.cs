namespace WorkoutNest.Infrastructure.Mongo.Entities;

public class Exercise
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string PrimaryMuscleGroup { get; set; }
    public string OtherMuscleGroup { get; set; }
    public string Equipment { get; set; }

    public Exercise(string id, string name, string primaryMuscleGroup, string otherMuscleGroup, string equipment)
    {
        Id = id;
        Name = name;
        PrimaryMuscleGroup = primaryMuscleGroup;
        OtherMuscleGroup = otherMuscleGroup;
        Equipment = equipment;
    }
}