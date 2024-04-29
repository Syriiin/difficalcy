using Difficalcy.Models;

namespace Difficalcy.Taiko.Models
{
    public record TaikoCalculation : Calculation<TaikoDifficulty, TaikoPerformance>
    {
        public double Accuracy { get; init; }
        public double Combo { get; init; }
    }
}
