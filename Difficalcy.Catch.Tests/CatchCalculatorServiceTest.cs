using Difficalcy.Catch.Models;
using Difficalcy.Catch.Services;
using Difficalcy.Models;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Catch.Tests;

public class CatchCalculatorServiceTest
    : CalculatorServiceTest<
        CatchScore,
        CatchDifficulty,
        CatchPerformance,
        CatchCalculation,
        CatchBeatmapDetails
    >
{
    protected override CalculatorService<
        CatchScore,
        CatchDifficulty,
        CatchPerformance,
        CatchCalculation,
        CatchBeatmapDetails
    > CalculatorService { get; } =
        new CatchCalculatorService(
            new InMemoryCache(),
            new TestBeatmapProvider(typeof(CatchCalculatorService).Assembly.GetName().Name)
        );

    [Theory]
    [InlineData(4.050546351620621, 164.57708668213735, "diffcalc-test", new string[] { })]
    [InlineData(5.169641126078549, 291.4348097171394, "diffcalc-test", new string[] { "DT" })]
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
        TestGetCalculationReturnsCorrectValues(6.61877502983358, 384.78709155438418, score);
    }

    [Fact]
    public async Task TestBeatmapDetails()
    {
        var beatmapId = "diffcalc-test";
        var beatmapDetails = await CalculatorService.GetBeatmapDetails(beatmapId);
        Assert.Equal("Unknown", beatmapDetails.Artist);
        Assert.Equal("Unknown", beatmapDetails.Title);
        Assert.Equal("Normal", beatmapDetails.DifficultyName);
        Assert.Equal("Unknown Creator", beatmapDetails.Author);
        Assert.Equal(127, beatmapDetails.MaxCombo);
        Assert.Equal(45250, beatmapDetails.Length);
        Assert.Equal(120, beatmapDetails.MinBPM);
        Assert.Equal(120, beatmapDetails.MaxBPM);
        Assert.Equal(120, beatmapDetails.CommonBPM);
        Assert.Equal(78, beatmapDetails.FruitCount);
        Assert.Equal(12, beatmapDetails.JuiceStreamCount);
        Assert.Equal(3, beatmapDetails.BananaShowerCount);
        Assert.Equal(4, beatmapDetails.CircleSize);
        Assert.Equal(8.3, beatmapDetails.ApproachRate, 4);
        Assert.Equal(5, beatmapDetails.DrainRate);
        Assert.Equal(1.6, beatmapDetails.BaseVelocity, 4);
        Assert.Equal(1, beatmapDetails.TickRate);
    }
}
