using System.Threading.Tasks;
using Difficalcy.Models;
using StackExchange.Redis;

namespace Difficalcy.Services
{
    public abstract class CalculatorService<TScore, TDifficulty, TPerformance, TCalculation>
        where TScore : Score
        where TDifficulty : Difficulty
        where TPerformance : Performance
        where TCalculation : Calculation<TDifficulty, TPerformance>
    {
        public abstract string RulesetName { get; }
        public abstract string CalculatorName { get; }
        public abstract string CalculatorPackage { get; }
        public abstract string CalculatorVersion { get; }

        public string CalculatorDiscriminator => $"{CalculatorPackage}:{CalculatorVersion}";

        private readonly IConnectionMultiplexer _redis;

        public CalculatorService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public abstract Task EnsureBeatmap(int beatmapId);

        public abstract (object, string) CalculateDifficulty(TScore score);

        public abstract TDifficulty GetDifficulty(object difficultyAttributes);

        public abstract object DeserialiseDifficultyAttributes(string difficultyAttributesJson);

        public abstract TPerformance CalculatePerformance(TScore score, object difficultyAttributes);

        public abstract TCalculation GetCalculation(TDifficulty difficulty, TPerformance performance);

        public async Task<TDifficulty> GetDifficulty(TScore score)
        {
            var difficultyAttributes = await GetDifficultyAttributes(score);
            return GetDifficulty(difficultyAttributes);
        }

        public async Task<TPerformance> GetPerformance(TScore score)
        {
            var difficultyAttributes = await GetDifficultyAttributes(score);
            return CalculatePerformance(score, difficultyAttributes);
        }

        public async Task<TCalculation> GetCalculation(TScore score)
        {
            var difficultyAttributes = await GetDifficultyAttributes(score);
            var difficulty = GetDifficulty(difficultyAttributes);
            var performance = CalculatePerformance(score, difficultyAttributes);
            return GetCalculation(difficulty, performance);
        }

        private async Task<object> GetDifficultyAttributes(TScore score)
        {
            await EnsureBeatmap(score.BeatmapId);

            var db = _redis.GetDatabase();
            var redisKey = $"{CalculatorDiscriminator}:{score.BeatmapId}:{score.Mods ?? 0}";
            var difficultyAttributesJson = await db.StringGetAsync(redisKey);

            object difficultyAttributes;
            if (difficultyAttributesJson.IsNull)
            {
                (difficultyAttributes, difficultyAttributesJson) = CalculateDifficulty(score);
                db.StringSet(redisKey, difficultyAttributesJson, flags: CommandFlags.FireAndForget);
            }
            else
            {
                difficultyAttributes = DeserialiseDifficultyAttributes(difficultyAttributesJson);
            }

            return difficultyAttributes;
        }
    }
}
