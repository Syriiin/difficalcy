using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Mania.Tests;

public class ManiaCalculatorServiceTest : CalculatorServiceTest<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation>
{
    protected override CalculatorService<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation> CalculatorService { get; } = new ManiaCalculatorService(new TestCache(), new TestBeatmapProvider("osu.Game.Rulesets.Mania"));

    [Theory]
    [InlineData(2.3493769750220914d, 45.71911573894849d, "diffcalc-test", 0)]
    [InlineData(2.797245912537965d, 68.73627121504641d, "diffcalc-test", 64)]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, int mods)
        => base.TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new ManiaScore { BeatmapId = beatmapId, Mods = mods });
}
