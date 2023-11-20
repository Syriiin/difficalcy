using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Mania.Tests;

public class ManiaCalculatorServiceTest : CalculatorServiceTest<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation>
{
    protected override CalculatorService<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation> CalculatorService => new ManiaCalculatorService(new DummyCache(), new TestBeatmapProvider("osu.Game.Rulesets.Mania"));

    [Theory]
    [InlineData(2.3449735700206298d, 43.846469224942766d, "diffcalc-test", 0)]
    [InlineData(2.7879104989252959d, 65.81066322316853d, "diffcalc-test", 64)]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, int mods)
        => base.TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new ManiaScore { BeatmapId = beatmapId, Mods = mods });
}
