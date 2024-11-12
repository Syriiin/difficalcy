using Difficalcy.Models;
using Difficalcy.Osu.Models;
using Difficalcy.Osu.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Osu.Tests;

public class OsuCalculatorServiceTest
    : CalculatorServiceTest<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation>
{
    protected override CalculatorService<
        OsuScore,
        OsuDifficulty,
        OsuPerformance,
        OsuCalculation
    > CalculatorService { get; } =
        new OsuCalculatorService(
            new InMemoryCache(),
            new TestBeatmapProvider(typeof(OsuCalculatorService).Assembly.GetName().Name)
        );

    [Theory]
    [InlineData(6.7171144000821119d, 291.34799376682508d, "diffcalc-test", new string[] { })]
    [InlineData(8.9825709931204205d, 717.13844713272601d, "diffcalc-test", new string[] { "DT" })]
    public void Test(
        double expectedDifficultyTotal,
        double expectedPerformanceTotal,
        string beatmapId,
        string[] mods
    ) =>
        TestGetCalculationReturnsCorrectValues(
            expectedDifficultyTotal,
            expectedPerformanceTotal,
            new OsuScore
            {
                BeatmapId = beatmapId,
                Mods = mods.Select(m => new Mod { Acronym = m }).ToArray(),
            }
        );

    [Fact]
    public void TestAllParameters()
    {
        var score = new OsuScore
        {
            BeatmapId = "diffcalc-test",
            Mods =
            [
                new Mod() { Acronym = "HD" },
                new Mod() { Acronym = "HR" },
                new Mod()
                {
                    Acronym = "DT",
                    Settings = new Dictionary<string, string> { { "speed_change", "2" } },
                },
                new Mod() { Acronym = "FL" },
            ],
            Combo = 200,
            Misses = 5,
            Mehs = 4,
            Oks = 3,
        };
        TestGetCalculationReturnsCorrectValues(12.418442356371395, 1415.202990027042, score);
    }

    [Fact]
    public void TestClassicMod()
    {
        var score = new OsuScore
        {
            BeatmapId = "diffcalc-test",
            Mods = [new Mod() { Acronym = "CL" }],
        };
        TestGetCalculationReturnsCorrectValues(6.7171144000821119d, 289.16416504218972, score);
    }
}
