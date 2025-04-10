using Difficalcy.Models;

namespace Difficalcy.Taiko.Models
{
    public record TaikoBeatmapDetails : BeatmapDetails
    {
        // Hit Objects
        public int HitCount { get; init; }
        public int DrumRollCount { get; init; }
        public int SwellCount { get; init; }

        // Difficulty Settings
        public double Accuracy { get; init; }
        public double DrainRate { get; init; }
    }
}
