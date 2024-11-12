using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;
using Difficalcy.Models;
using Difficalcy.Services;
using Difficalcy.Tests;

namespace Difficalcy.Mania.Tests;

public class ManiaCalculatorServiceTest : CalculatorServiceTest<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation>
{
    protected override CalculatorService<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation> CalculatorService { get; } = new ManiaCalculatorService(new InMemoryCache(), new TestBeatmapProvider(typeof(ManiaCalculatorService).Assembly.GetName().Name));

    [Theory]
    [InlineData(2.3493769750220914d, 45.76140071089439d, "diffcalc-test", new string[] { })]
    [InlineData(2.797245912537965d, 68.79984443279172d, "diffcalc-test", new string[] { "DT" })]
    public void Test(double expectedDifficultyTotal, double expectedPerformanceTotal, string beatmapId, string[] mods)
        => TestGetCalculationReturnsCorrectValues(expectedDifficultyTotal, expectedPerformanceTotal, new ManiaScore { BeatmapId = beatmapId, Mods = mods.Select(m => new Mod { Acronym = m }).ToArray() });

    [Fact]
    public void TestAllParameters()
    {
        var score = new ManiaScore
        {
            BeatmapId = "diffcalc-test",
            Mods = [
                new Mod()
                {
                    Acronym = "DT",
                    Settings = new Dictionary<string, string>
                    {
                        { "speed_change", "2" }
                    }
                }
            ],
            Misses = 5,
            Mehs = 4,
            Oks = 3,
            Goods = 2,
            Greats = 1,
        };
        TestGetCalculationReturnsCorrectValues(3.3252153148972425, 64.408516282383957, score);
    }
}
