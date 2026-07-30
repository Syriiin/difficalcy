using Difficalcy.Catch.Models;
using Difficalcy.Catch.Services;
using Difficalcy.Models;
using Difficalcy.Services;

namespace Difficalcy.Catch.Tests;

public class CatchCalculatorServiceTest
{
    private CalculatorService<
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

    [Fact]
    public async Task Test()
    {
        var score = new CatchScore { BeatmapId = "diffcalc-test", Mods = [] };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(4.039861734717169, calculation.Difficulty.Total, 4);
        Assert.Equal(163.70914311938117, calculation.Performance.Total, 4);
        Assert.Equal(1, calculation.Accuracy, 4);
        Assert.Equal(127, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestWithDT()
    {
        var score = new CatchScore
        {
            BeatmapId = "diffcalc-test",
            Mods = [new Mod() { Acronym = "DT" }],
        };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(5.1527173897800873, calculation.Difficulty.Total, 4);
        Assert.Equal(289.52836279061012, calculation.Performance.Total, 4);
        Assert.Equal(1, calculation.Accuracy, 4);
        Assert.Equal(127, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestAllParameters()
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

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(6.6156692905339396, calculation.Difficulty.Total, 4);
        Assert.Equal(384.42585375147621, calculation.Performance.Total, 4);
        Assert.Equal(0.95833333333333337, calculation.Accuracy, 4);
        Assert.Equal(100, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestAllParametersClassicMod()
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
                new Mod() { Acronym = "CL" },
            ],
            Combo = 100,
            Misses = 5,
            LargeDroplets = 18,
            SmallDroplets = 200,
        };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(6.6156692905339396, calculation.Difficulty.Total, 4);
        Assert.Equal(384.42585375147621, calculation.Performance.Total, 4);
        Assert.Equal(0.95833333333333337, calculation.Accuracy, 4);
        Assert.Equal(100, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
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
