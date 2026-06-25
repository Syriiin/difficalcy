namespace Difficalcy.Taiko.Models;

public record TaikoDifficultyAttributesDto
{
    public double StarRating { get; init; }
    public int MaxCombo { get; init; }
    public double MonoStaminaFactor { get; init; }
    public double StaminaDifficulty { get; init; }
    public double RhythmDifficulty { get; init; }
    public double ColourDifficulty { get; init; }
    public double ConsistencyFactor { get; init; }
}
