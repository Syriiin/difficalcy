using Difficalcy.Models;
using Difficalcy.Services;
using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;

namespace Difficalcy.Taiko.Tests;

public class TaikoCalculatorServiceTest
{
    private CalculatorService<
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

    [Fact]
    public async Task Test()
    {
        var score = new TaikoScore { BeatmapId = "diffcalc-test", Mods = [] };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(3.3190848563395079, calculation.Difficulty.Total, 4);
        Assert.Equal(168.87232101873195, calculation.Performance.Total, 4);
        Assert.Equal(1, calculation.Accuracy, 4);
        Assert.Equal(200, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestWithDT()
    {
        var score = new TaikoScore
        {
            BeatmapId = "diffcalc-test",
            Mods = [new Mod() { Acronym = "DT" }],
        };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(4.4551414906554987, calculation.Difficulty.Total, 4);
        Assert.Equal(324.15878408465358, calculation.Performance.Total, 4);
        Assert.Equal(1, calculation.Accuracy, 4);
        Assert.Equal(200, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestAllParameters()
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

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(6.22607274618289, calculation.Difficulty.Total, 4);
        Assert.Equal(568.88629422631971, calculation.Performance.Total, 4);
        Assert.Equal(0.96750000000000003, calculation.Accuracy, 4);
        Assert.Equal(150, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestAllParametersClassicMod()
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
                new Mod() { Acronym = "CL" },
            ],
            Combo = 150,
            Misses = 5,
            Oks = 3,
        };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(6.22607274618289, calculation.Difficulty.Total, 4);
        Assert.Equal(568.88629422631971, calculation.Performance.Total, 4);
        Assert.Equal(0.96750000000000003, calculation.Accuracy, 4);
        Assert.Equal(150, calculation.Combo, 4);

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
