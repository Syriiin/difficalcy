using Difficalcy.Models;
using Difficalcy.Services;
using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;
using Difficalcy.Tests;

namespace Difficalcy.Taiko.Tests;

public class TaikoCalculatorServiceTest
    : CalculatorServiceTest<TaikoScore, TaikoDifficulty, TaikoPerformance, TaikoCalculation>
{
    protected override CalculatorService<
        TaikoScore,
        TaikoDifficulty,
        TaikoPerformance,
        TaikoCalculation
    > CalculatorService { get; } =
        new TaikoCalculatorService(
            new InMemoryCache(),
            new TestBeatmapProvider(typeof(TaikoCalculatorService).Assembly.GetName().Name)
        );

    [Theory]
    [InlineData(3.092021259435121d, 137.80325540434842d, "diffcalc-test", new string[] { })]
    [InlineData(4.0789820318081444d, 248.8310568362074d, "diffcalc-test", new string[] { "DT" })]
    public void Test(
        double expectedDifficultyTotal,
        double expectedPerformanceTotal,
        string beatmapId,
        string[] mods
    ) =>
        TestGetCalculationReturnsCorrectValues(
            expectedDifficultyTotal,
            expectedPerformanceTotal,
            new TaikoScore
            {
                BeatmapId = beatmapId,
                Mods = mods.Select(m => new Mod { Acronym = m }).ToArray(),
            }
        );

    [Fact]
    public void TestAllParameters()
    {
        var score = new TaikoScore
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
            Combo = 150,
            Misses = 5,
            Oks = 3,
        };
        TestGetCalculationReturnsCorrectValues(4.922364692298034, 359.95282202016443, score);
    }
}
