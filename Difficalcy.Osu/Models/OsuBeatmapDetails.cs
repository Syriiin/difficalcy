using Difficalcy.Models;

namespace Difficalcy.Osu.Models
{
    public record OsuBeatmapDetails : BeatmapDetails
    {
        // Hit Objects
        public int CircleCount { get; init; }
        public int SliderCount { get; init; }
        public int SpinnerCount { get; init; }
        public int SliderTickCount { get; init; }

        // Difficulty Settings
        public double CircleSize { get; init; }
        public double ApproachRate { get; init; }
        public double Accuracy { get; init; }
        public double DrainRate { get; init; }
    }
}
