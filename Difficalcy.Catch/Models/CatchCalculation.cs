using Difficalcy.Models;

namespace Difficalcy.Catch.Models
{
    public record CatchCalculation : Calculation<CatchDifficulty, CatchPerformance>
    {
        public double Accuracy { get; init; }
        public double Combo { get; init; }
    }
}
