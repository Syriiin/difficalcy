namespace Difficalcy.Tests;

using Difficalcy.Models;
using Difficalcy.Services;

public abstract class CalculatorServiceTest<TScore, TDifficulty, TPerformance, TCalculation>
        where TScore : Score
        where TDifficulty : Difficulty
        where TPerformance : Performance
        where TCalculation : Calculation<TDifficulty, TPerformance>
{
    protected abstract CalculatorService<TScore, TDifficulty, TPerformance, TCalculation> CalculatorService { get; }

    public async void TestGetCalculationReturnsCorrectValues(double expectedDifficultyTotal, double expectedPerformanceTotal, TScore score)
    {
        var calculation = await CalculatorService.GetCalculation(score);

        Assert.Equal(expectedDifficultyTotal, calculation.Difficulty.Total, 4);
        Assert.Equal(expectedPerformanceTotal, calculation.Performance.Total, 4);

        var calculationFromCache = await CalculatorService.GetCalculation(score);

        Assert.Equal(calculationFromCache.Difficulty.Total, calculation.Difficulty.Total);
        Assert.Equal(calculationFromCache.Performance.Total, calculation.Performance.Total);
    }
}
