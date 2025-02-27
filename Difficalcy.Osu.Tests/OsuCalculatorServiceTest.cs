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
    [InlineData(6.733134719196091, 293.62857973470625, "diffcalc-test", new string[] { })]
    [InlineData(9.677974635300163, 884.3332110176514, "diffcalc-test", new string[] { "DT" })]
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
            SliderTails = 2,
            SliderTicks = 1,
        };
        TestGetCalculationReturnsCorrectValues(13.89060549007635, 1718.493680115238, score);
    }

    [Fact]
    public void TestAllParametersClassicMod()
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
                new Mod() { Acronym = "CL" },
            ],
            Combo = 200,
            Misses = 5,
            Mehs = 4,
            Oks = 3,
            SliderTails = 2,
            SliderTicks = 1,
        };
        TestGetCalculationReturnsCorrectValues(13.89060549007635, 2012.0980668102025, score);
    }
}
