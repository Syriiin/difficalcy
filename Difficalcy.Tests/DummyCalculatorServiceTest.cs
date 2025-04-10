namespace Difficalcy.Tests;

using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Services;

public class DummyCalculatorServiceTest
    : CalculatorServiceTest<
        DummyScore,
        DummyDifficulty,
        DummyPerformance,
        DummyCalculation,
        DummyBeatmapDetails
    >
{
    protected override CalculatorService<
        DummyScore,
        DummyDifficulty,
        DummyPerformance,
        DummyCalculation,
        DummyBeatmapDetails
    > CalculatorService { get; } = new DummyCalculatorService(new InMemoryCache());

    [Theory]
    [InlineData(15, 1500, "test 1", new string[] { "150" })]
    [InlineData(10, 1000, "test 2", new string[] { "25", "75" })]
    public void Test(
        double expectedDifficultyTotal,
        double expectedPerformanceTotal,
        string beatmapId,
        string[] mods
    ) =>
        TestGetCalculationReturnsCorrectValues(
            expectedDifficultyTotal,
            expectedPerformanceTotal,
            new DummyScore
            {
                BeatmapId = beatmapId,
                Mods = mods.Select(m => new Mod { Acronym = m }).ToArray(),
                Points = 100,
            }
        );

    [Fact]
    public async Task TestGetCalculationBatchReturnsCorrectValuesInOrder()
    {
        // values are intentionally in a random order to ensure unique beatmap grouping doesnt break return ordering
        var scores = new[]
        {
            new DummyScore
            {
                BeatmapId = "test 1",
                Mods = [new Mod() { Acronym = "200" }],
                Points = 200,
            }, // 3
            new DummyScore
            {
                BeatmapId = "test 2",
                Mods = [new Mod() { Acronym = "300" }],
                Points = 100,
            }, // 4
            new DummyScore
            {
                BeatmapId = "test 2",
                Mods = [new Mod() { Acronym = "300" }],
                Points = 200,
            }, // 5
            new DummyScore
            {
                BeatmapId = "test 3",
                Mods = [new Mod() { Acronym = "500" }],
                Points = 200,
            }, // 9
            new DummyScore
            {
                BeatmapId = "test 2",
                Mods = [new Mod() { Acronym = "400" }],
                Points = 200,
            }, // 7
            new DummyScore
            {
                BeatmapId = "test 1",
                Mods = [new Mod() { Acronym = "200" }],
                Points = 100,
            }, // 2
            new DummyScore
            {
                BeatmapId = "test 3",
                Mods = [new Mod() { Acronym = "600" }],
                Points = 100,
            }, // 10
            new DummyScore
            {
                BeatmapId = "test 2",
                Mods = [new Mod() { Acronym = "400" }],
                Points = 100,
            }, // 6
            new DummyScore
            {
                BeatmapId = "test 3",
                Mods = [new Mod() { Acronym = "500" }],
                Points = 100,
            }, // 8
            new DummyScore
            {
                BeatmapId = "test 1",
                Mods = [new Mod() { Acronym = "100" }],
                Points = 200,
            }, // 1
            new DummyScore
            {
                BeatmapId = "test 3",
                Mods = [new Mod() { Acronym = "600" }],
                Points = 200,
            }, // 11
            new DummyScore
            {
                BeatmapId = "test 1",
                Mods = [new Mod() { Acronym = "100" }],
                Points = 100,
            }, // 0
        };

        var calculations = (await CalculatorService.GetCalculationBatch(scores)).ToArray();

        Assert.Equal(12, calculations.Length);

        Assert.Equal(20, calculations[0].Difficulty.Total); // 3
        Assert.Equal(4000, calculations[0].Performance.Total);

        Assert.Equal(30, calculations[1].Difficulty.Total); // 4
        Assert.Equal(3000, calculations[1].Performance.Total);

        Assert.Equal(30, calculations[2].Difficulty.Total); // 5
        Assert.Equal(6000, calculations[2].Performance.Total);

        Assert.Equal(50, calculations[3].Difficulty.Total); // 9
        Assert.Equal(10000, calculations[3].Performance.Total);

        Assert.Equal(40, calculations[4].Difficulty.Total); // 7
        Assert.Equal(8000, calculations[4].Performance.Total);

        Assert.Equal(20, calculations[5].Difficulty.Total); // 2
        Assert.Equal(2000, calculations[5].Performance.Total);

        Assert.Equal(60, calculations[6].Difficulty.Total); // 10
        Assert.Equal(6000, calculations[6].Performance.Total);

        Assert.Equal(40, calculations[7].Difficulty.Total); // 6
        Assert.Equal(4000, calculations[7].Performance.Total);

        Assert.Equal(50, calculations[8].Difficulty.Total); // 8
        Assert.Equal(5000, calculations[8].Performance.Total);

        Assert.Equal(10, calculations[9].Difficulty.Total); // 1
        Assert.Equal(2000, calculations[9].Performance.Total);

        Assert.Equal(60, calculations[10].Difficulty.Total); // 11
        Assert.Equal(12000, calculations[10].Performance.Total);

        Assert.Equal(10, calculations[11].Difficulty.Total); // 0
        Assert.Equal(1000, calculations[11].Performance.Total);
    }

    [Fact]
    public async Task TestGetBeatmapDetailsReturnsCorrectValues()
    {
        var beatmapId = "test 1";
        var beatmapDetails = await CalculatorService.GetBeatmapDetails(beatmapId);

        Assert.Equal("Dummy artist", beatmapDetails.Artist);
        Assert.Equal("Dummy title", beatmapDetails.Title);
        Assert.Equal("Dummy diff", beatmapDetails.DifficultyName);
        Assert.Equal("Dummy author", beatmapDetails.Author);
        Assert.Equal(100, beatmapDetails.MaxCombo);
        Assert.Equal(200, beatmapDetails.Length);
        Assert.Equal(300, beatmapDetails.MinBPM);
        Assert.Equal(400, beatmapDetails.MaxBPM);
        Assert.Equal(500, beatmapDetails.CommonBPM);
        Assert.Equal(600, beatmapDetails.BaseVelocity);
        Assert.Equal(700, beatmapDetails.TickRate);
    }
}

/// <summary>
/// A dummy calculator service implementation that calculates difficulty as mods (casted from string to double) / 10 and performance as difficulty * points
/// </summary>
public class DummyCalculatorService(ICache cache)
    : CalculatorService<
        DummyScore,
        DummyDifficulty,
        DummyPerformance,
        DummyCalculation,
        DummyBeatmapDetails
    >(cache)
{
    public override CalculatorInfo Info =>
        new()
        {
            RulesetName = "Dummy",
            CalculatorName = "Dummy calculator",
            CalculatorPackage = "DummyCalculatorPackage",
            CalculatorVersion = "DummyCalculatorVersion",
            CalculatorUrl = $"not.a.real.url",
        };

    protected override DummyBeatmapDetails GetBeatmapDetailsSync(string beatmapId)
    {
        return new DummyBeatmapDetails
        {
            Artist = "Dummy artist",
            Title = "Dummy title",
            DifficultyName = "Dummy diff",
            Author = "Dummy author",
            MaxCombo = 100,
            Length = 200,
            MinBPM = 300,
            MaxBPM = 400,
            CommonBPM = 500,
            BaseVelocity = 600,
            TickRate = 700,
        };
    }

    protected override (object, string) CalculateDifficultyAttributes(string beatmapId, Mod[] mods)
    {
        var difficulty = mods.Sum(m => double.Parse(m.Acronym)) / 10;
        return (difficulty, difficulty.ToString());
    }

    protected override DummyCalculation CalculatePerformance(
        DummyScore score,
        object difficultyAttributes
    ) =>
        new()
        {
            Difficulty = new DummyDifficulty() { Total = (double)difficultyAttributes },
            Performance = new DummyPerformance()
            {
                Total = (double)difficultyAttributes * score.Points,
            },
        };

    protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson) =>
        double.Parse(difficultyAttributesJson);

    protected override Task EnsureBeatmap(string beatmapId) => Task.FromResult(true);
}

public record DummyScore : Score
{
    public int Points { get; init; }
}

public record DummyDifficulty : Difficulty { }

public record DummyPerformance : Performance { }

public record DummyCalculation : Calculation<DummyDifficulty, DummyPerformance> { }

public record DummyBeatmapDetails : BeatmapDetails { }
