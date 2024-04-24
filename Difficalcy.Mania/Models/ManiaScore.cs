using Difficalcy.Models;

namespace Difficalcy.Mania.Models
{
    public record ManiaScore : Score
    {
        public double? Accuracy { get; init; }
        public int? Misses { get; init; }
        public int? Mehs { get; init; }
        public int? Oks { get; init; }
        public int? Goods { get; init; }
        public int? Greats { get; init; }
    }
}
