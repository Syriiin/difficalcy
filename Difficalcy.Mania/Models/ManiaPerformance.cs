using Difficalcy.Models;

namespace Difficalcy.Mania.Models
{
    public record ManiaPerformance : Performance
    {
        public double Difficulty { get; init; }
    }
}
