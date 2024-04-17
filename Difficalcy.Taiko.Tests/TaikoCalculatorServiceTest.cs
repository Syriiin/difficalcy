using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Taiko.Tests;

public class TaikoCalculatorServiceTest : CalculatorServiceTest<TaikoScore, TaikoDifficulty, TaikoPerformance, TaikoCalculation>
{
    protected override CalculatorService<TaikoScore, TaikoDifficulty, TaikoPerformance, TaikoCalculation> CalculatorService { get; } = new TaikoCalculatorService(new InMemoryCache(), new TestBeatmapProvider("osu.Game.Rulesets.Taiko"));

    [Theory]
    [InlineData(3.0920212594351191d, 108.45361131093136d, "diffcalc-test", 0)]
    [InlineData(4.0789820318081444d, 197.40372508926697d, "diffcalc-test", 64)]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, int mods)
        => base.TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new TaikoScore { BeatmapId = beatmapId, Mods = mods });
}
