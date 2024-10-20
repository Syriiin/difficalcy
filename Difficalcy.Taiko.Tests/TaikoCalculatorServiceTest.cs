using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Taiko.Tests;

public class TaikoCalculatorServiceTest : CalculatorServiceTest<TaikoScore, TaikoDifficulty, TaikoPerformance, TaikoCalculation>
{
    protected override CalculatorService<TaikoScore, TaikoDifficulty, TaikoPerformance, TaikoCalculation> CalculatorService { get; } = new TaikoCalculatorService(new InMemoryCache(), new TestBeatmapProvider(typeof(TaikoCalculatorService).Assembly.GetName().Name));

    [Theory]
    [InlineData(3.092021259435121d, 137.80325540434842d, "diffcalc-test", 0)]
    [InlineData(4.0789820318081444d, 248.8310568362074d, "diffcalc-test", 64)]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, int mods)
        => TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new TaikoScore { BeatmapId = beatmapId, Mods = mods });

    [Fact]
    public void TestAllParameters()
    {
        var score = new TaikoScore
        {
            BeatmapId = "diffcalc-test",
            Mods = 80, // HR, DT
            Combo = 150,
            Misses = 5,
            Oks = 3,
        };
        TestGetCalculationReturnsCorrectValues(4.0789820318081444d, 240.24516772998618d, score);
    }
}
