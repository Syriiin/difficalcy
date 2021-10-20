namespace Difficalcy.Models
{
    public abstract record Calculation<TDifficulty, TPerformance>
        where TDifficulty : Difficulty
        where TPerformance : Performance
    {
        public TDifficulty Difficulty { get; init; }
        public TPerformance Performance { get; init; }
    }
}
