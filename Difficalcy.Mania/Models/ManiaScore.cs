using System.ComponentModel.DataAnnotations;
using Difficalcy.Models;

namespace Difficalcy.Mania.Models
{
    public record ManiaScore : Score
    {
        [Range(0, int.MaxValue)]
        public int Misses { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Mehs { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Oks { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Goods { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Greats { get; init; } = 0;
    }
}
