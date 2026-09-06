using System.Text.Json;
using Difficalcy.Catch.Models;

namespace Difficalcy.Catch.Tests;

public class CatchNonFiniteSerializationTest
{
    [Fact]
    public void DifficultyDtoWithNaNSerialises()
    {
        var dto = new CatchDifficultyAttributesDto { StarRating = double.NaN, MaxCombo = 1 };

        var json = JsonSerializer.Serialize(
            dto,
            CatchJsonContext.Default.CatchDifficultyAttributesDto
        );

        Assert.Contains("\"NaN\"", json);
        var deserialised = JsonSerializer.Deserialize(
            json,
            CatchJsonContext.Default.CatchDifficultyAttributesDto
        );
        Assert.True(double.IsNaN(deserialised!.StarRating));
    }

    [Fact]
    public void CalculationWithNaNSerialises()
    {
        var calculation = new CatchCalculation
        {
            Difficulty = new CatchDifficulty { Total = double.NaN },
            Performance = new CatchPerformance { Total = double.NaN },
        };

        var json = JsonSerializer.Serialize(calculation, CatchJsonContext.Default.CatchCalculation);

        Assert.Contains("\"NaN\"", json);
        var deserialised = JsonSerializer.Deserialize(
            json,
            CatchJsonContext.Default.CatchCalculation
        );
        Assert.True(double.IsNaN(deserialised!.Difficulty.Total));
        Assert.True(double.IsNaN(deserialised.Performance.Total));
    }
}
