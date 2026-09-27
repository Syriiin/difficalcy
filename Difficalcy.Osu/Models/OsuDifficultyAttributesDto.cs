namespace Difficalcy.Osu.Models;

public record OsuDifficultyAttributesDto
{
    public double StarRating { get; init; }
    public int MaxCombo { get; init; }
    public double AimDifficulty { get; init; }
    public double SpeedDifficulty { get; init; }
    public double SpeedNoteCount { get; init; }
    public double FlashlightDifficulty { get; init; }
    public double ReadingDifficulty { get; init; }
    public double SliderFactor { get; init; }
    public double AimDifficultSliderCount { get; init; }
    public double AimDifficultStrainCount { get; init; }
    public double SpeedDifficultStrainCount { get; init; }
    public double ReadingDifficultNoteCount { get; init; }
    public int HitCircleCount { get; init; }
    public int SliderCount { get; init; }
    public int SpinnerCount { get; init; }
    public double AimTopWeightedSliderFactor { get; init; }
    public double SpeedTopWeightedSliderFactor { get; init; }
    public double NestedScorePerObject { get; init; }
    public double LegacyScoreBaseMultiplier { get; init; }
    public double MaximumLegacyComboScore { get; init; }
}
