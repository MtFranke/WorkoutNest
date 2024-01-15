using WorkoutNest.API.Workouts;

namespace WorkoutNest.API.Tests;

public class GainTests
{
    [Fact]
    public void Gain_GetPercentageGain_ShouldGiveCorrectPercentageGain()
    {
       var res = Gain.GetPercentageGain(9, 10);
       res.ShouldBe(11.11);
    }
}