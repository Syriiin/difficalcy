using Difficalcy.Models;

namespace Difficalcy.Taiko.Models
{
    public record TaikoScore : Score
    {
        public double? Accuracy { get; init; }
        public int? Combo { get; init; }
        public int? Misses { get; init; }
        public int? Oks { get; init; }
    }
}
