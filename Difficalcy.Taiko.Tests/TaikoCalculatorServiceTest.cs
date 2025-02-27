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
    [InlineData(3.3055544732491895, 135.44034861603984, "diffcalc-test", new string[] { })]
    [InlineData(4.447257361895736, 272.94981653131504, "diffcalc-test", new string[] { "DT" })]
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
        TestGetCalculationReturnsCorrectValues(6.216091072305756, 451.45173560370836, score);
    }
}
