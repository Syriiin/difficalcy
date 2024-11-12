using Difficalcy.Catch.Models;
using Difficalcy.Catch.Services;
using Difficalcy.Models;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Catch.Tests;

public class CatchCalculatorServiceTest
    : CalculatorServiceTest<CatchScore, CatchDifficulty, CatchPerformance, CatchCalculation>
{
    protected override CalculatorService<
        CatchScore,
        CatchDifficulty,
        CatchPerformance,
        CatchCalculation
    > CalculatorService { get; } =
        new CatchCalculatorService(
            new InMemoryCache(),
            new TestBeatmapProvider(typeof(CatchCalculatorService).Assembly.GetName().Name)
        );

    [Theory]
    [InlineData(4.0505463516206195d, 164.5770866821372d, "diffcalc-test", new string[] { })]
    [InlineData(5.1696411260785498d, 291.43480971713944d, "diffcalc-test", new string[] { "DT" })]
    public void Test(
        double expectedDifficultyTotal,
        double expectedPerformanceTotal,
        string beatmapId,
        string[] mods
    ) =>
        TestGetCalculationReturnsCorrectValues(
            expectedDifficultyTotal,
            expectedPerformanceTotal,
            new CatchScore
            {
                BeatmapId = beatmapId,
                Mods = mods.Select(m => new Mod { Acronym = m }).ToArray(),
            }
        );

    [Fact]
    public void TestAllParameters()
    {
        var score = new CatchScore
        {
            BeatmapId = "diffcalc-test",
            Mods =
            [
                new Mod() { Acronym = "HR" },
                new Mod()
                {
                    Acronym = "DT",
                    Settings = new Dictionary<string, string> { { "speed_change", "2" } },
                },
            ],
            Combo = 100,
            Misses = 5,
            LargeDroplets = 18,
            SmallDroplets = 200,
        };
        TestGetCalculationReturnsCorrectValues(6.9017468199992278, 375.74458599075302, score);
    }
}
