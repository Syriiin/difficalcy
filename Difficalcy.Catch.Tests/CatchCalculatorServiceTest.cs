using Difficalcy.Catch.Models;
using Difficalcy.Catch.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Catch.Tests;

public class CatchCalculatorServiceTest : CalculatorServiceTest<CatchScore, CatchDifficulty, CatchPerformance, CatchCalculation>
{
    protected override CalculatorService<CatchScore, CatchDifficulty, CatchPerformance, CatchCalculation> CalculatorService { get; } = new CatchCalculatorService(new InMemoryCache(), new TestBeatmapProvider(typeof(CatchCalculatorService).Assembly.GetName().Name));

    [Theory]
    [InlineData(4.0505463516206195d, 164.5770866821372d, "diffcalc-test", 0)]
    [InlineData(5.1696411260785498d, 291.43480971713944d, "diffcalc-test", 64)]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, int mods)
        => base.TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new CatchScore { BeatmapId = beatmapId, Mods = mods });

    [Fact]
    public void TestAllParameters()
    {
        var score = new CatchScore
        {
            BeatmapId = "diffcalc-test",
            Mods = 80, // HR, DT
            Combo = 100,
            Misses = 5,
            LargeDroplets = 18,
            SmallDroplets = 200,
        };
        base.TestGetCalculationReturnsCorrectValues(5.739025024925009d, 241.19384779497875d, score);
    }
}
