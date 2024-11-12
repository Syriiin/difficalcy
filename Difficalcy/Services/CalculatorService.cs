using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Difficalcy.Models;

namespace Difficalcy.Services
{
    public abstract class CalculatorService<TScore, TDifficulty, TPerformance, TCalculation>(ICache cache)
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

        /// <summary>
        /// Ensures the beatmap with the given ID is available locally.
        /// </summary>
        protected abstract Task EnsureBeatmap(string beatmapId);

        /// <summary>
        /// Runs the difficulty calculator and returns the difficulty attributes as both an object and JSON serialised string.
        /// </summary>
        protected abstract (object, string) CalculateDifficultyAttributes(string beatmapId, Mod[] mods);

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
            var difficultyAttributes = await GetDifficultyAttributes(score.BeatmapId, score.Mods);
            return CalculatePerformance(score, difficultyAttributes);
        }

        public async Task<IEnumerable<TCalculation>> GetCalculationBatch(TScore[] scores)
        {
            var scoresWithIndex = scores.Select((score, index) => (score, index));
            var uniqueBeatmapGroups = scoresWithIndex.GroupBy(scoreWithIndex => (scoreWithIndex.score.BeatmapId, GetModString(scoreWithIndex.score.Mods)));

            var calculationGroups = await Task.WhenAll(uniqueBeatmapGroups.Select(async group =>
            {
                var scores = group.Select(scoreWithIndex => scoreWithIndex.score);
                return group.Select(scoreWithIndex => scoreWithIndex.index).Zip(await GetUniqueBeatmapCalculationBatch(group.Key.BeatmapId, scores.First().Mods, scores));
            }));

            return calculationGroups.SelectMany(group => group).OrderBy(group => group.First).Select(group => group.Second);
        }

        private async Task<IEnumerable<TCalculation>> GetUniqueBeatmapCalculationBatch(string beatmapId, Mod[] mods, IEnumerable<TScore> scores)
        {
            var difficultyAttributes = await GetDifficultyAttributes(beatmapId, mods);
            return scores.AsParallel().AsOrdered().Select(score => CalculatePerformance(score, difficultyAttributes));
        }

        private async Task<object> GetDifficultyAttributes(string beatmapId, Mod[] mods)
        {
            await EnsureBeatmap(beatmapId);

            var db = cache.GetDatabase();
            var redisKey = GetRedisKey(beatmapId, mods);
            var difficultyAttributesJson = await db.GetAsync(redisKey);

            object difficultyAttributes;
            if (difficultyAttributesJson == null)
            {
                (difficultyAttributes, difficultyAttributesJson) = CalculateDifficultyAttributes(beatmapId, mods);
                db.Set(redisKey, difficultyAttributesJson);
            }
            else
            {
                difficultyAttributes = DeserialiseDifficultyAttributes(difficultyAttributesJson);
            }

            return difficultyAttributes;
        }

        private string GetRedisKey(string beatmapId, Mod[] mods) => $"difficalcy:{CalculatorDiscriminator}:{beatmapId}:{GetModString(mods)}";

        private static string GetModString(Mod[] mods) => string.Join(",", mods.OrderBy(mod => mod.Acronym).Select(mod => mod.ToString()));
    }
}
