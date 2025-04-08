namespace Difficalcy.Tests;

using Difficalcy.Models;
using Difficalcy.Services;

public abstract class CalculatorServiceTest<
    TScore,
    TDifficulty,
    TPerformance,
    TCalculation,
    TBeatmapDetails
>
    where TScore : Score
    where TDifficulty : Difficulty
    where TPerformance : Performance
    where TCalculation : Calculation<TDifficulty, TPerformance>
    where TBeatmapDetails : BeatmapDetails
{
    protected abstract CalculatorService<
        TScore,
        TDifficulty,
        TPerformance,
        TCalculation,
        TBeatmapDetails
    > CalculatorService { get; }

    public async void TestGetCalculationReturnsCorrectValues(
        double expectedDifficultyTotal,
        double expectedPerformanceTotal,
        TScore score
    )
    {
        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(expectedDifficultyTotal, calculation.Difficulty.Total, 4);
        Assert.Equal(expectedPerformanceTotal, calculation.Performance.Total, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculation, calculationFromCache);
    }
}
