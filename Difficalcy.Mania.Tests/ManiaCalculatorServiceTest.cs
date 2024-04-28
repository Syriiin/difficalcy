using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Mania.Tests;

public class ManiaCalculatorServiceTest : CalculatorServiceTest<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation>
{
    protected override CalculatorService<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation> CalculatorService { get; } = new ManiaCalculatorService(new InMemoryCache(), new TestBeatmapProvider(typeof(ManiaCalculatorService).Assembly.GetName().Name));

    [Theory]
    [InlineData(2.3493769750220914d, 45.76140071089439d, "diffcalc-test", 0)]
    [InlineData(2.797245912537965d, 68.79984443279172d, "diffcalc-test", 64)]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, int mods)
        => base.TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new ManiaScore { BeatmapId = beatmapId, Mods = mods });

    [Fact]
    public void TestAllParameters()
    {
        var score = new ManiaScore
        {
            BeatmapId = "diffcalc-test",
            Mods = 64, // DT
            Misses = 5,
            Mehs = 4,
            Oks = 3,
            Goods = 2,
            Greats = 1,
        };
        base.TestGetCalculationReturnsCorrectValues(2.797245912537965d, 43.17076331130473d, score);
    }
}
