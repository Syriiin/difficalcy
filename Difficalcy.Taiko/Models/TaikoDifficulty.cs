using Difficalcy.Models;

namespace Difficalcy.Taiko.Models
{
    public record TaikoDifficulty : Difficulty
    {
        public double Stamina { get; init; }
        public double Rhythm { get; init; }
        public double Colour { get; init; }
    }
}
