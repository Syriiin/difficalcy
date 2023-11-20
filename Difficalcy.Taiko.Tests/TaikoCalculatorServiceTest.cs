using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Taiko.Tests;

public class TaikoCalculatorServiceTest : CalculatorServiceTest<TaikoScore, TaikoDifficulty, TaikoPerformance, TaikoCalculation>
{
    protected override CalculatorService<TaikoScore, TaikoDifficulty, TaikoPerformance, TaikoCalculation> CalculatorService => new TaikoCalculatorService(new DummyCache(), new TestBeatmapProvider("osu.Game.Rulesets.Taiko"));

    [Theory]
    [InlineData(2.2420075288523802d, 100.38891498669157d, "diffcalc-test", 0)]
    [InlineData(3.134084469440479d, 165.39807330636233d, "diffcalc-test", 64)]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, int mods)
        => base.TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new TaikoScore { BeatmapId = beatmapId, Mods = mods });
}
