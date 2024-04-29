using System.Threading.Tasks;
using Difficalcy.Models;

namespace Difficalcy.Services
{
    public abstract class CalculatorService<TScore, TDifficulty, TPerformance, TCalculation>
        where TScore : Score
        where TDifficulty : Difficulty
        where TPerformance : Performance
        where TCalculation : Calculation<TDifficulty, TPerformance>
    {
        /// <summary>
        /// A set of information describing the calculator.
        /// </summary>
        public abstract CalculatorInfo Info { get; }

        /// <summary>
        /// A unique discriminator for this calculator.
        /// Should be unique for calculator that might return differing results.
        /// </summary>
        public string CalculatorDiscriminator => $"{Info.CalculatorPackage}:{Info.CalculatorVersion}";

        private readonly ICache _cache;

        public CalculatorService(ICache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Ensures the beatmap with the given ID is available locally.
        /// </summary>
        protected abstract Task EnsureBeatmap(string beatmapId);

        /// <summary>
        /// Runs the difficulty calculator and returns the difficulty attributes as both an object and JSON serialised string.
        /// </summary>
        protected abstract (object, string) CalculateDifficultyAttributes(TScore score);

        /// <summary>
        /// Returns the deserialised object for a given JSON serialised difficulty attributes object.
        /// </summary>
        protected abstract object DeserialiseDifficultyAttributes(string difficultyAttributesJson);

        /// <summary>
        /// Runs the performance calculator on a given score with pre-calculated difficulty attributes and returns the performance.
        /// </summary>
        protected abstract TCalculation CalculatePerformance(TScore score, object difficultyAttributes);

        /// <summary>
        /// Returns the calculation of a given score.
        /// </summary>
        public async Task<TCalculation> GetCalculation(TScore score)
        {
            var difficultyAttributes = await GetDifficultyAttributes(score);
            return CalculatePerformance(score, difficultyAttributes);
        }

        private async Task<object> GetDifficultyAttributes(TScore score)
        {
            await EnsureBeatmap(score.BeatmapId);

            var db = _cache.GetDatabase();
            var redisKey = $"difficalcy:{CalculatorDiscriminator}:{score.BeatmapId}:{score.Mods}";
            var difficultyAttributesJson = await db.GetAsync(redisKey);

            object difficultyAttributes;
            if (difficultyAttributesJson == null)
            {
                (difficultyAttributes, difficultyAttributesJson) = CalculateDifficultyAttributes(score);
                db.Set(redisKey, difficultyAttributesJson);
            }
            else
            {
                difficultyAttributes = DeserialiseDifficultyAttributes(difficultyAttributesJson);
            }

            return difficultyAttributes;
        }
    }
}
