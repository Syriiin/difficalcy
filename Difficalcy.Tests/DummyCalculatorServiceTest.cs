namespace Difficalcy.Tests;

using Difficalcy.Models;
using Difficalcy.Services;

public class DummyCalculatorServiceTest : CalculatorServiceTest<DummyScore, DummyDifficulty, DummyPerformance, DummyCalculation>
{
    protected override CalculatorService<DummyScore, DummyDifficulty, DummyPerformance, DummyCalculation> CalculatorService { get; } = new DummyCalculatorService(new InMemoryCache());

    [Theory]
    [InlineData(15, 1500, "test 1", 150)]
    [InlineData(10, 1000, "test 2", 100)]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, int mods)
        => TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new DummyScore { BeatmapId = beatmapId, Mods = mods, Points = 100 });

    [Fact]
    public async Task TestGetCalculationBatchReturnsCorrectValuesInOrder()
    {
        // values are intentionally in a random order to ensure unique beatmap grouping doesnt break return ordering
        var scores = new[]
        {
            new DummyScore { BeatmapId = "test 1", Mods = 200, Points = 200 }, // 3
            new DummyScore { BeatmapId = "test 2", Mods = 300, Points = 100 }, // 4
            new DummyScore { BeatmapId = "test 2", Mods = 300, Points = 200 }, // 5
            new DummyScore { BeatmapId = "test 3", Mods = 500, Points = 200 }, // 9
            new DummyScore { BeatmapId = "test 2", Mods = 400, Points = 200 }, // 7
            new DummyScore { BeatmapId = "test 1", Mods = 200, Points = 100 }, // 2
            new DummyScore { BeatmapId = "test 3", Mods = 600, Points = 100 }, // 10
            new DummyScore { BeatmapId = "test 2", Mods = 400, Points = 100 }, // 6
            new DummyScore { BeatmapId = "test 3", Mods = 500, Points = 100 }, // 8
            new DummyScore { BeatmapId = "test 1", Mods = 100, Points = 200 }, // 1
            new DummyScore { BeatmapId = "test 3", Mods = 600, Points = 200 }, // 11
            new DummyScore { BeatmapId = "test 1", Mods = 100, Points = 100 }, // 0
        };

        var calculations = (await CalculatorService.GetCalculationBatch(scores)).ToArray();

        Assert.Equal(12, calculations.Length);

        Assert.Equal(20, calculations[0].Difficulty.Total);         // 3
        Assert.Equal(4000, calculations[0].Performance.Total);

        Assert.Equal(30, calculations[1].Difficulty.Total);         // 4
        Assert.Equal(3000, calculations[1].Performance.Total);

        Assert.Equal(30, calculations[2].Difficulty.Total);         // 5
        Assert.Equal(6000, calculations[2].Performance.Total);

        Assert.Equal(50, calculations[3].Difficulty.Total);         // 9
        Assert.Equal(10000, calculations[3].Performance.Total);

        Assert.Equal(40, calculations[4].Difficulty.Total);         // 7
        Assert.Equal(8000, calculations[4].Performance.Total);

        Assert.Equal(20, calculations[5].Difficulty.Total);         // 2
        Assert.Equal(2000, calculations[5].Performance.Total);

        Assert.Equal(60, calculations[6].Difficulty.Total);         // 10
        Assert.Equal(6000, calculations[6].Performance.Total);

        Assert.Equal(40, calculations[7].Difficulty.Total);         // 6
        Assert.Equal(4000, calculations[7].Performance.Total);

        Assert.Equal(50, calculations[8].Difficulty.Total);         // 8
        Assert.Equal(5000, calculations[8].Performance.Total);

        Assert.Equal(10, calculations[9].Difficulty.Total);         // 1
        Assert.Equal(2000, calculations[9].Performance.Total);

        Assert.Equal(60, calculations[10].Difficulty.Total);        // 11
        Assert.Equal(12000, calculations[10].Performance.Total);

        Assert.Equal(10, calculations[11].Difficulty.Total);        // 0
        Assert.Equal(1000, calculations[11].Performance.Total);
    }
}

/// <summary>
/// A dummy calculator service implementation that calculates difficulty as (beatmap id + mods) / 10 and performance as difficulty * 100
/// </summary>
public class DummyCalculatorService(ICache cache) : CalculatorService<DummyScore, DummyDifficulty, DummyPerformance, DummyCalculation>(cache)
{
    public override CalculatorInfo Info =>
        new()
        {
            RulesetName = "Dummy",
            CalculatorName = "Dummy calculator",
            CalculatorPackage = "DummyCalculatorPackage",
            CalculatorVersion = "DummyCalculatorVersion",
            CalculatorUrl = $"not.a.real.url"
        };

    protected override (object, string) CalculateDifficultyAttributes(string beatmapId, int mods)
    {
        var difficulty = mods / 10.0;
        return (difficulty, difficulty.ToString());
    }

    protected override DummyCalculation CalculatePerformance(DummyScore score, object difficultyAttributes) =>
        new()
        {
            Difficulty = new DummyDifficulty() { Total = (double)difficultyAttributes },
            Performance = new DummyPerformance() { Total = (double)difficultyAttributes * score.Points }
        };

    protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson) =>
        double.Parse(difficultyAttributesJson);

    protected override Task EnsureBeatmap(string beatmapId) =>
        Task.FromResult(true);
}

public record DummyScore : Score
{
    public int Points { get; init; }
}
public record DummyDifficulty : Difficulty { }
public record DummyPerformance : Performance { }
public record DummyCalculation : Calculation<DummyDifficulty, DummyPerformance> { }
