using Difficalcy.Models;
using Difficalcy.Services;
using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;
using Difficalcy.Tests;

namespace Difficalcy.Taiko.Tests;

public class TaikoCalculatorServiceTest
    : CalculatorServiceTest<
        TaikoScore,
        TaikoDifficulty,
        TaikoPerformance,
        TaikoCalculation,
        TaikoBeatmapDetails
    >
{
    protected override CalculatorService<
        TaikoScore,
        TaikoDifficulty,
        TaikoPerformance,
        TaikoCalculation,
        TaikoBeatmapDetails
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

    [Fact]
    public async Task TestBeatmapDetails()
    {
        var beatmapId = "diffcalc-test";
        var beatmapDetails = await CalculatorService.GetBeatmapDetails(beatmapId);
        Assert.Equal("Unknown", beatmapDetails.Artist);
        Assert.Equal("Unknown", beatmapDetails.Title);
        Assert.Equal("Normal", beatmapDetails.DifficultyName);
        Assert.Equal("Unknown Creator", beatmapDetails.Author);
        Assert.Equal(200, beatmapDetails.MaxCombo);
        Assert.Equal(53000, beatmapDetails.Length);
        Assert.Equal(120, beatmapDetails.MinBPM);
        Assert.Equal(120, beatmapDetails.MaxBPM);
        Assert.Equal(120, beatmapDetails.CommonBPM);
        Assert.Equal(200, beatmapDetails.HitCount);
        Assert.Equal(30, beatmapDetails.DrumRollCount);
        Assert.Equal(8, beatmapDetails.SwellCount);
        Assert.Equal(7, beatmapDetails.Accuracy);
        Assert.Equal(5, beatmapDetails.DrainRate);
        Assert.Equal(1.6, beatmapDetails.BaseVelocity, 4);
        Assert.Equal(1, beatmapDetails.TickRate);
    }
}
