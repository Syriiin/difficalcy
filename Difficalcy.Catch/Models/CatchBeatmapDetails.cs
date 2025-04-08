using Difficalcy.Models;

namespace Difficalcy.Catch.Models
{
    public record CatchBeatmapDetails : BeatmapDetails
    {
        // Hit Objects
        public int FruitCount { get; init; }
        public int JuiceStreamCount { get; init; }
        public int BananaShowerCount { get; init; }

        // Difficulty Settings
        public double CircleSize { get; init; }
        public double ApproachRate { get; init; }
        public double DrainRate { get; init; }
    }
}
