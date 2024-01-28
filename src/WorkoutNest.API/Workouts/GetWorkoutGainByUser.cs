using FastEndpoints;
using MongoDB.Driver;
using WorkoutNest.Infrastructure.Mongo;
using WorkoutNest.Infrastructure.Mongo.Entities;

namespace WorkoutNest.API.Workouts;

public class GetWorkoutGainByUser : EndpointWithoutRequest<Summary>
{
    private readonly IMongoWrapper _mongoWrapper;

    public override void Configure()
    {
        Get("/workout/gains");
    }
    

    public GetWorkoutGainByUser(IMongoWrapper mongoWrapper)
    {
        _mongoWrapper = mongoWrapper;
    }
    
    public override async Task HandleAsync(CancellationToken c)
    {
        var userId  = User.Claims.Single(x => x.Type == "user_id").Value;
        var workouts =  _mongoWrapper.GetCollection<Workout>(Collections.WorkoutsCollection);
        var userWorkouts = await (await workouts.FindAsync(workout => workout.UserId == userId, cancellationToken: c))
            .ToListAsync(c);
        var lastWorkout = userWorkouts[^1];
        var secondToLast = userWorkouts
            .Where(workout => workout.WorkoutSchemaId == lastWorkout.WorkoutSchemaId)
            .OrderByDescending(workout => workout.Date).Skip(1).FirstOrDefault();

        List<Gain> gains = new List<Gain>();
        double totalWorkotVolume = 0;
        double totalPrevWorkotVolume = 0;
        foreach (var lw in lastWorkout.Exercises)
        {
           var exerciseExist = secondToLast.Exercises.Any(x => x.ExercisesId == lw.ExercisesId);
           if (exerciseExist)
           {
               var sets = secondToLast.Exercises
                   .Where(x => x.ExercisesId == lw.ExercisesId)
                   .Select(x => x.Sets)
                   .Single();
               double previousVolume = 0;
               double currentVolume = 0;
               foreach (var set in sets)
               {
                   previousVolume += set.Reps * set.Weight;
               }

               foreach (var set in lw.Sets)
               {
                   currentVolume += set.Reps * set.Weight;

               }
               
               gains.Add(new Gain(lw.ExercisesId, previousVolume, currentVolume));
               totalWorkotVolume += currentVolume;
           }
        }

        foreach (var stl in secondToLast.Exercises)
        {
            foreach (var set in stl.Sets)
            {
                totalPrevWorkotVolume += set.Reps * set.Weight;
            }
        }

        var summ = new Summary(totalPrevWorkotVolume, totalWorkotVolume, gains);
        await SendAsync(summ, cancellation: c);
    }

}

public class Gain
{
    public string ExerciseId { get; set; }
    public double Volume { get; set; }
    
    public double Increase { get; set; }
    
    public Gain(string exerciseId, double totalPreviousVolume, double currentVolume)
    {
        ExerciseId = exerciseId;
        Volume = totalPreviousVolume;
        Increase = GetPercentageGain(totalPreviousVolume, currentVolume);
    }

    public static double GetPercentageGain(double startValue, double finalValue)
    {
        return  Math.Round(((finalValue - startValue) / startValue) * 100, 2);
    }
    
    
}

public class Summary
{ 
    public double TotalVolume { get; set; }
    
    public double Increase { get; set; }
    
    public IEnumerable<Gain> Volumes { get; set; }

    public Summary(double totalPreviousVolume, double volume, List<Gain> gains)
    {
        Volumes = gains;
        TotalVolume = totalPreviousVolume;
        Increase = Gain.GetPercentageGain(totalPreviousVolume, volume);
    }
}