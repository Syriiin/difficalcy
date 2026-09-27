using Difficalcy.Models;
using Difficalcy.Osu.Models;
using Difficalcy.Osu.Services;
using Difficalcy.Services;

namespace Difficalcy.Osu.Tests;

public class OsuCalculatorServiceTest
{
    private CalculatorService<
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

    [Fact]
    public async Task Test()
    {
        var score = new OsuScore { BeatmapId = "diffcalc-test", Mods = [] };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(6.5243230054514676, calculation.Difficulty.Total, 4);
        Assert.Equal(291.15100073619107, calculation.Performance.Total, 4);
        Assert.Equal(1, calculation.Accuracy, 4);
        Assert.Equal(239, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestWithDT()
    {
        var score = new OsuScore
        {
            BeatmapId = "diffcalc-test",
            Mods = [new Mod() { Acronym = "DT" }],
        };

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(9.4677694877983463, calculation.Difficulty.Total, 4);
        Assert.Equal(878.04911849488235, calculation.Performance.Total, 4);
        Assert.Equal(1, calculation.Accuracy, 4);
        Assert.Equal(239, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestAllParameters()
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

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(14.27904896620441, calculation.Difficulty.Total, 4);
        Assert.Equal(1380.5431667352334, calculation.Performance.Total, 4);
        Assert.Equal(0.77180004483299702, calculation.Accuracy, 4);
        Assert.Equal(200, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }

    [Fact]
    public async Task TestAllParametersClassicMod()
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

        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(14.27904896620441, calculation.Difficulty.Total, 4);
        Assert.Equal(1702.8430403970876, calculation.Performance.Total, 4);
        Assert.Equal(0.91666666666666663, calculation.Accuracy, 4);
        Assert.Equal(200, calculation.Combo, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
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
