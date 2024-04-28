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
        => base.TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new DummyScore { BeatmapId = beatmapId, Mods = mods });
}

/// <summary>
/// A dummy calculator service implementation that calculates difficulty as (beatmap id + mods) / 10 and performance as difficulty * 100
/// </summary>
public class DummyCalculatorService : CalculatorService<DummyScore, DummyDifficulty, DummyPerformance, DummyCalculation>
{
    public DummyCalculatorService(ICache cache) : base(cache)
    {
    }

    public override CalculatorInfo Info =>
        new CalculatorInfo
        {
            RulesetName = "Dummy",
            CalculatorName = "Dummy calculator",
            CalculatorPackage = "DummyCalculatorPackage",
            CalculatorVersion = "DummyCalculatorVersion",
            CalculatorUrl = $"not.a.real.url"
        };

    protected override (object, string) CalculateDifficultyAttributes(DummyScore score)
    {
        var difficulty = score.Mods / 10.0;
        return (difficulty, difficulty.ToString());
    }

    protected override DummyPerformance CalculatePerformance(DummyScore score, object difficultyAttributes) =>
        new DummyPerformance { Total = (double)difficultyAttributes * 100 };

    protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson) =>
        double.Parse(difficultyAttributesJson);

    protected override Task<bool> EnsureBeatmap(string beatmapId) =>
        Task.FromResult(true);

    protected override DummyCalculation GetCalculation(DummyDifficulty difficulty, DummyPerformance performance) =>
        new DummyCalculation
        {
            Difficulty = difficulty,
            Performance = performance
        };

    protected override DummyDifficulty GetDifficultyFromDifficultyAttributes(object difficultyAttributes) =>
        new DummyDifficulty
        {
            Total = (double)difficultyAttributes
        };
}

public record DummyScore : Score { }
public record DummyDifficulty : Difficulty { }
public record DummyPerformance : Performance { }
public record DummyCalculation : Calculation<DummyDifficulty, DummyPerformance> { }
