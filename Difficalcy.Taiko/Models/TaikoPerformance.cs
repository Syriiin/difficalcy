using Difficalcy.Models;

namespace Difficalcy.Taiko.Models
{
    public record TaikoPerformance : Performance
    {
        public double Strain { get; init; }
        public double Accuracy { get; init; }
    }
}
