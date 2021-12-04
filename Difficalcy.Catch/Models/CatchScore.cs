using Difficalcy.Models;

namespace Difficalcy.Catch.Models
{
    public record CatchScore : Score
    {
        public double? Accuracy { get; init; }
        public int? Combo { get; init; }
        public int? Misses { get; init; }
        public int? TinyDroplets { get; init; }
        public int? Droplets { get; init; }
    }
}
