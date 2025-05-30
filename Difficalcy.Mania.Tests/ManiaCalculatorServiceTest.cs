using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;
using Difficalcy.Models;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Mania.Tests;

public class ManiaCalculatorServiceTest
    : CalculatorServiceTest<
        ManiaScore,
        ManiaDifficulty,
        ManiaPerformance,
        ManiaCalculation,
        ManiaBeatmapDetails
    >
{
    protected override CalculatorService<
        ManiaScore,
        ManiaDifficulty,
        ManiaPerformance,
        ManiaCalculation,
        ManiaBeatmapDetails
    > CalculatorService { get; } =
        new ManiaCalculatorService(
            new InMemoryCache(),
            new TestBeatmapProvider(typeof(ManiaCalculatorService).Assembly.GetName().Name)
        );

    [Theory]
    [InlineData(2.3493769750220914, 45.76140071089439, "diffcalc-test", new string[] { })]
    [InlineData(2.797245912537965, 68.79984443279172, "diffcalc-test", new string[] { "DT" })]
    public void Test(
        double expectedDifficultyTotal,
        double expectedPerformanceTotal,
        string beatmapId,
        string[] mods
    ) =>
        TestGetCalculationReturnsCorrectValues(
            expectedDifficultyTotal,
            expectedPerformanceTotal,
            new ManiaScore
            {
                BeatmapId = beatmapId,
                Mods = mods.Select(m => new Mod { Acronym = m }).ToArray(),
            }
        );

    [Fact]
    public void TestAllParameters()
    {
        var score = new ManiaScore
        {
            BeatmapId = "diffcalc-test",
            Mods =
            [
                new Mod()
                {
                    Acronym = "DT",
                    Settings = new Dictionary<string, string> { { "speed_change", "2" } },
                },
            ],
            Misses = 5,
            Mehs = 4,
            Oks = 3,
            Goods = 2,
            Greats = 1,
        };
        TestGetCalculationReturnsCorrectValues(3.3252153148972425, 64.40851628238396, score);
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
        Assert.Equal(151, beatmapDetails.MaxCombo);
        Assert.Equal(30500, beatmapDetails.Length);
        Assert.Equal(120, beatmapDetails.MinBPM);
        Assert.Equal(120, beatmapDetails.MaxBPM);
        Assert.Equal(120, beatmapDetails.CommonBPM);
        Assert.Equal(123, beatmapDetails.NoteCount);
        Assert.Equal(14, beatmapDetails.HoldNoteCount);
        Assert.Equal(4, beatmapDetails.KeyCount);
        Assert.Equal(7, beatmapDetails.Accuracy);
        Assert.Equal(5, beatmapDetails.DrainRate);
        Assert.Equal(1.6, beatmapDetails.BaseVelocity, 4);
        Assert.Equal(1, beatmapDetails.TickRate);
    }
}
