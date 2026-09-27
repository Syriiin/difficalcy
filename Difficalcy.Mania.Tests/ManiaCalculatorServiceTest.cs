using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;
using Difficalcy.Models;
using Difficalcy.Services;

namespace Difficalcy.Mania.Tests;

public class ManiaCalculatorServiceTest
{
    private CalculatorService<
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

    [Fact]
    public async Task Test()
    {
        var score = new ManiaScore { BeatmapId = "diffcalc-test", Mods = [] };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(2.3493769750220914, calculation.Difficulty.Total, 4);
        Assert.Equal(45.76140071089439, calculation.Performance.Total, 4);
        Assert.Equal(1, calculation.Accuracy, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestWithDT()
    {
        var score = new ManiaScore
        {
            BeatmapId = "diffcalc-test",
            Mods = [new Mod() { Acronym = "DT" }],
        };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(2.797245912537965, calculation.Difficulty.Total, 4);
        Assert.Equal(68.79984443279172, calculation.Performance.Total, 4);
        Assert.Equal(1, calculation.Accuracy, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestAllParameters()
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

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(3.3252153148972425, calculation.Difficulty.Total, 4);
        Assert.Equal(64.40851628238396, calculation.Performance.Total, 4);
        Assert.Equal(0.92671805450005429, calculation.Accuracy, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestAllParametersClassicMod()
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
                new Mod() { Acronym = "CL" },
            ],
            Misses = 5,
            Mehs = 4,
            Oks = 3,
            Goods = 2,
            Greats = 1,
        };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(3.3252153148972425, calculation.Difficulty.Total, 4);
        Assert.Equal(60.445155625089356, calculation.Performance.Total, 4);
        Assert.Equal(0.91970802919708028, calculation.Accuracy, 4);

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
