using System.Text.Json;
using Difficalcy.Taiko.Models;

namespace Difficalcy.Taiko.Tests;

public class TaikoNonFiniteSerializationTest
{
    [Fact]
    public void DifficultyDtoWithNaNSerialises()
    {
        var dto = new TaikoDifficultyAttributesDto { StarRating = double.NaN, MaxCombo = 1 };

        var json = JsonSerializer.Serialize(
            dto,
            TaikoJsonContext.Default.TaikoDifficultyAttributesDto
        );

        Assert.Contains("\"NaN\"", json);
        var deserialised = JsonSerializer.Deserialize(
            json,
            TaikoJsonContext.Default.TaikoDifficultyAttributesDto
        );
        Assert.True(double.IsNaN(deserialised!.StarRating));
    }

    [Fact]
    public void CalculationWithNaNSerialises()
    {
        var calculation = new TaikoCalculation
        {
            Difficulty = new TaikoDifficulty { Total = double.NaN },
            Performance = new TaikoPerformance { Total = double.NaN },
        };

        var json = JsonSerializer.Serialize(calculation, TaikoJsonContext.Default.TaikoCalculation);

        Assert.Contains("\"NaN\"", json);
        var deserialised = JsonSerializer.Deserialize(
            json,
            TaikoJsonContext.Default.TaikoCalculation
        );
        Assert.True(double.IsNaN(deserialised!.Difficulty.Total));
        Assert.True(double.IsNaN(deserialised.Performance.Total));
    }
}
