using System.Text.Json;
using Difficalcy.Mania.Models;

namespace Difficalcy.Mania.Tests;

public class ManiaNonFiniteSerializationTest
{
    [Fact]
    public void DifficultyDtoWithNaNSerialises()
    {
        var dto = new ManiaDifficultyAttributesDto { StarRating = double.NaN, MaxCombo = 1 };

        var json = JsonSerializer.Serialize(
            dto,
            ManiaJsonContext.Default.ManiaDifficultyAttributesDto
        );

        Assert.Contains("\"NaN\"", json);
        var deserialised = JsonSerializer.Deserialize(
            json,
            ManiaJsonContext.Default.ManiaDifficultyAttributesDto
        );
        Assert.True(double.IsNaN(deserialised!.StarRating));
    }

    [Fact]
    public void CalculationWithNaNSerialises()
    {
        var calculation = new ManiaCalculation
        {
            Difficulty = new ManiaDifficulty { Total = double.NaN },
            Performance = new ManiaPerformance { Total = double.NaN },
        };

        var json = JsonSerializer.Serialize(calculation, ManiaJsonContext.Default.ManiaCalculation);

        Assert.Contains("\"NaN\"", json);
        var deserialised = JsonSerializer.Deserialize(
            json,
            ManiaJsonContext.Default.ManiaCalculation
        );
        Assert.True(double.IsNaN(deserialised!.Difficulty.Total));
        Assert.True(double.IsNaN(deserialised.Performance.Total));
    }
}
