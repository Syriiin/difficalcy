using Difficalcy.Models;

namespace Difficalcy.Mania.Models
{
    public record ManiaScore : Score
    {
        public int? TotalScore { get; init; }
    }
}
