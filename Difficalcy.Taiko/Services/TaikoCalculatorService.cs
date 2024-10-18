using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Services;
using Difficalcy.Taiko.Models;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Taiko;
using osu.Game.Rulesets.Taiko.Difficulty;
using osu.Game.Rulesets.Taiko.Objects;
using osu.Game.Scoring;

namespace Difficalcy.Taiko.Services
{
    public class TaikoCalculatorService(ICache cache, IBeatmapProvider beatmapProvider) : CalculatorService<TaikoScore, TaikoDifficulty, TaikoPerformance, TaikoCalculation>(cache)
    {
        private readonly IBeatmapProvider _beatmapProvider = beatmapProvider;
        private TaikoRuleset TaikoRuleset { get; } = new TaikoRuleset();

        public override CalculatorInfo Info
        {
            get
            {
                var packageName = Assembly.GetAssembly(typeof(TaikoRuleset)).GetName().Name;
                var packageVersion = Assembly.GetAssembly(typeof(TaikoRuleset)).GetName().Version.ToString();
                return new CalculatorInfo
                {
                    RulesetName = TaikoRuleset.Description,
                    CalculatorName = "Official osu!taiko",
                    CalculatorPackage = packageName,
                    CalculatorVersion = packageVersion,
                    CalculatorUrl = $"https://nuget.org/packages/ppy.{packageName}/{packageVersion}"
                };
            }
        }

        protected override async Task EnsureBeatmap(string beatmapId)
        {
            await _beatmapProvider.EnsureBeatmap(beatmapId);
        }

        protected override (object, string) CalculateDifficultyAttributes(string beatmapId, int bitMods)
        {
            var workingBeatmap = GetWorkingBeatmap(beatmapId);
            var mods = TaikoRuleset.ConvertFromLegacyMods((LegacyMods)bitMods).ToArray();

            var difficultyCalculator = TaikoRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as TaikoDifficultyAttributes;

            // Serialising anonymous object with same names because some properties can't be serialised, and the built-in JsonProperty fields aren't on all required fields
            return (difficultyAttributes, JsonSerializer.Serialize(new
            {
                difficultyAttributes.StarRating,
                difficultyAttributes.MaxCombo,
                difficultyAttributes.StaminaDifficulty,
                difficultyAttributes.RhythmDifficulty,
                difficultyAttributes.ColourDifficulty,
                difficultyAttributes.PeakDifficulty,
                difficultyAttributes.GreatHitWindow,
                difficultyAttributes.OkHitWindow
            }));
        }

        protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<TaikoDifficultyAttributes>(difficultyAttributesJson);
        }

        protected override TaikoCalculation CalculatePerformance(TaikoScore score, object difficultyAttributes)
        {
            var taikoDifficultyAttributes = (TaikoDifficultyAttributes)difficultyAttributes;

            var workingBeatmap = GetWorkingBeatmap(score.BeatmapId);
            var mods = TaikoRuleset.ConvertFromLegacyMods((LegacyMods)score.Mods).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(TaikoRuleset.RulesetInfo, mods);

            var hitResultCount = beatmap.HitObjects.OfType<Hit>().Count();
            var combo = score.Combo ?? hitResultCount;
            var statistics = GetHitResults(hitResultCount, score.Misses, score.Oks);
            var accuracy = CalculateAccuracy(statistics);

            var scoreInfo = new ScoreInfo(beatmap.BeatmapInfo, TaikoRuleset.RulesetInfo)
            {
                Accuracy = accuracy,
                MaxCombo = combo,
                Statistics = statistics,
                Mods = mods
            };

            var performanceCalculator = TaikoRuleset.CreatePerformanceCalculator();
            var performanceAttributes = performanceCalculator.Calculate(scoreInfo, taikoDifficultyAttributes) as TaikoPerformanceAttributes;

            return new TaikoCalculation()
            {
                Difficulty = GetDifficultyFromDifficultyAttributes(taikoDifficultyAttributes),
                Performance = GetPerformanceFromPerformanceAttributes(performanceAttributes),
                Accuracy = accuracy,
                Combo = combo
            };
        }

        private CalculatorWorkingBeatmap GetWorkingBeatmap(string beatmapId)
        {
            using var beatmapStream = _beatmapProvider.GetBeatmapStream(beatmapId);
            return new CalculatorWorkingBeatmap(TaikoRuleset, beatmapStream);
        }

        private static Dictionary<HitResult, int> GetHitResults(int hitResultCount, int countMiss, int countOk)
        {
            var countGreat = hitResultCount - countOk - countMiss;

            return new Dictionary<HitResult, int>
            {
                { HitResult.Great, countGreat },
                { HitResult.Ok, countOk },
                { HitResult.Meh, 0 },
                { HitResult.Miss, countMiss }
            };
        }

        private static double CalculateAccuracy(Dictionary<HitResult, int> statistics)
        {
            var countGreat = statistics[HitResult.Great];
            var countOk = statistics[HitResult.Ok];
            var countMiss = statistics[HitResult.Miss];
            var total = countGreat + countOk + countMiss;

            if (total == 0)
                return 1;

            return (double)((2 * countGreat) + countOk) / (2 * total);
        }

        private static TaikoDifficulty GetDifficultyFromDifficultyAttributes(TaikoDifficultyAttributes difficultyAttributes)
        {
            return new TaikoDifficulty()
            {
                Total = difficultyAttributes.StarRating,
                Stamina = difficultyAttributes.StaminaDifficulty,
                Rhythm = difficultyAttributes.RhythmDifficulty,
                Colour = difficultyAttributes.ColourDifficulty
            };
        }

        private static TaikoPerformance GetPerformanceFromPerformanceAttributes(TaikoPerformanceAttributes performanceAttributes)
        {
            return new TaikoPerformance()
            {
                Total = performanceAttributes.Total,
                Difficulty = performanceAttributes.Difficulty,
                Accuracy = performanceAttributes.Accuracy
            };
        }
    }
}
