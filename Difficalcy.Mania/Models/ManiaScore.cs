using System.ComponentModel.DataAnnotations;
using Difficalcy.Models;

namespace Difficalcy.Mania.Models
{
    public record ManiaScore : Score
    {
        [Range(0, 1)]
        public double? Accuracy { get; init; }

        [Range(0, int.MaxValue)]
        public int? Misses { get; init; }

        [Range(0, int.MaxValue)]
        public int? Mehs { get; init; }

        [Range(0, int.MaxValue)]
        public int? Oks { get; init; }

        [Range(0, int.MaxValue)]
        public int? Goods { get; init; }

        [Range(0, int.MaxValue)]
        public int? Greats { get; init; }
    }
}
