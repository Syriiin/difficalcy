using Difficalcy.Models;
using Difficalcy.Osu.Models;
using Difficalcy.Osu.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Osu.Tests;

public class OsuCalculatorServiceTest
    : CalculatorServiceTest<
        OsuScore,
        OsuDifficulty,
        OsuPerformance,
        OsuCalculation,
        OsuBeatmapDetails
    >
{
    protected override CalculatorService<
        OsuScore,
        OsuDifficulty,
        OsuPerformance,
        OsuCalculation,
        OsuBeatmapDetails
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

    [Fact]
    public async Task TestGetBeatmapDetails()
    {
        var beatmapId = "diffcalc-test";
        var beatmapDetails = await CalculatorService.GetBeatmapDetails(beatmapId);
        Assert.Equal("Unknown", beatmapDetails.Artist);
        Assert.Equal("Unknown", beatmapDetails.Title);
        Assert.Equal("Normal", beatmapDetails.DifficultyName);
        Assert.Equal("Unknown Creator", beatmapDetails.Author);
        Assert.Equal(239, beatmapDetails.MaxCombo);
        Assert.Equal(102500, beatmapDetails.Length);
        Assert.Equal(120, beatmapDetails.MinBPM);
        Assert.Equal(120, beatmapDetails.MaxBPM);
        Assert.Equal(120, beatmapDetails.CommonBPM);
        Assert.Equal(79, beatmapDetails.CircleCount);
        Assert.Equal(33, beatmapDetails.SliderCount);
        Assert.Equal(12, beatmapDetails.SpinnerCount);
        Assert.Equal(82, beatmapDetails.SliderTickCount);
        Assert.Equal(4, beatmapDetails.CircleSize);
        Assert.Equal(8.3, beatmapDetails.ApproachRate, 4);
        Assert.Equal(7, beatmapDetails.Accuracy);
        Assert.Equal(5, beatmapDetails.DrainRate);
        Assert.Equal(1.6, beatmapDetails.BaseVelocity, 4);
        Assert.Equal(1, beatmapDetails.TickRate);
    }
}
