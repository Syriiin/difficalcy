using Difficalcy.Osu.Models;
using Difficalcy.Osu.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Osu.Tests;

public class OsuCalculatorServiceTest : CalculatorServiceTest<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation>
{
    protected override CalculatorService<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation> CalculatorService => new OsuCalculatorService(new DummyCache(), new TestBeatmapProvider("osu.Game.Rulesets.Osu"));

    [Theory]
    [InlineData(6.5867229481955389d, 273.31622171900824d, "diffcalc-test", 0)]
    [InlineData(8.2730989071947896d, 567.4273062942285d, "diffcalc-test", 64)]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, int mods)
        => base.TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new OsuScore { BeatmapId = beatmapId, Mods = mods });
}
