using Difficalcy.Models;

namespace Difficalcy.Mania.Models
{
    public record ManiaPerformance : Performance
    {
        public double Strain { get; init; }
        public double Accuracy { get; init; }
    }
}
