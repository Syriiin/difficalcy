using Difficalcy.Models;

namespace Difficalcy.Mania.Models
{
    public record ManiaCalculation : Calculation<ManiaDifficulty, ManiaPerformance>
    {
        public double Accuracy { get; init; }
    }
}
