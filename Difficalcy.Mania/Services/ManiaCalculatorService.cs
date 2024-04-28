using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Mania.Models;
using Difficalcy.Models;
using Difficalcy.Services;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Mania.Difficulty;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace Difficalcy.Mania.Services
{
    public class ManiaCalculatorService : CalculatorService<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation>
    {
        private readonly IBeatmapProvider _beatmapProvider;
        private ManiaRuleset ManiaRuleset { get; } = new ManiaRuleset();

        public override CalculatorInfo Info
        {
            get
            {
                var packageName = Assembly.GetAssembly(typeof(ManiaRuleset)).GetName().Name;
                var packageVersion = Assembly.GetAssembly(typeof(ManiaRuleset)).GetName().Version.ToString();
                return new CalculatorInfo
                {
                    RulesetName = ManiaRuleset.Description,
                    CalculatorName = "Official osu!mania",
                    CalculatorPackage = packageName,
                    CalculatorVersion = packageVersion,
                    CalculatorUrl = $"https://nuget.org/packages/ppy.{packageName}/{packageVersion}"
                };
            }
        }

        public ManiaCalculatorService(ICache cache, IBeatmapProvider beatmapProvider) : base(cache)
        {
            _beatmapProvider = beatmapProvider;
        }

        protected override async Task EnsureBeatmap(string beatmapId)
        {
            await _beatmapProvider.EnsureBeatmap(beatmapId);
        }

        protected override (object, string) CalculateDifficultyAttributes(ManiaScore score)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = ManiaRuleset.ConvertFromLegacyMods((LegacyMods)score.Mods).ToArray();

            var difficultyCalculator = ManiaRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as ManiaDifficultyAttributes;

            // Serialising anonymous object with same names because some properties can't be serialised, and the built-in JsonProperty fields aren't on all required fields
            return (difficultyAttributes, JsonSerializer.Serialize(new
            {
                StarRating = difficultyAttributes.StarRating,
                MaxCombo = difficultyAttributes.MaxCombo,
                GreatHitWindow = difficultyAttributes.GreatHitWindow
            }));
        }

        protected override ManiaDifficulty GetDifficultyFromDifficultyAttributes(object difficultyAttributes)
        {
            var maniaDifficultyAttributes = (ManiaDifficultyAttributes)difficultyAttributes;
            return new ManiaDifficulty()
            {
                Total = maniaDifficultyAttributes.StarRating
            };
        }

        protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<ManiaDifficultyAttributes>(difficultyAttributesJson);
        }

        protected override ManiaPerformance CalculatePerformance(ManiaScore score, object difficultyAttributes)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = ManiaRuleset.ConvertFromLegacyMods((LegacyMods)score.Mods).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(ManiaRuleset.RulesetInfo, mods);

            var hitObjectCount = beatmap.HitObjects.Count;
            var holdNoteTailCount = beatmap.HitObjects.OfType<HoldNote>().Count();
            var statistics = getHitResults(hitObjectCount + holdNoteTailCount, score.Misses, score.Mehs, score.Oks, score.Goods, score.Greats);
            var accuracy = calculateAccuracy(statistics);

            var scoreInfo = new ScoreInfo(beatmap.BeatmapInfo, ManiaRuleset.RulesetInfo)
            {
                Accuracy = accuracy,
                MaxCombo = 0,
                Statistics = statistics,
                Mods = mods,
            };

            var performanceCalculator = ManiaRuleset.CreatePerformanceCalculator();
            var performanceAttributes = performanceCalculator.Calculate(scoreInfo, (ManiaDifficultyAttributes)difficultyAttributes) as ManiaPerformanceAttributes;

            return new ManiaPerformance()
            {
                Total = performanceAttributes.Total,
                Difficulty = performanceAttributes.Difficulty
            };
        }

        protected override ManiaCalculation GetCalculation(ManiaDifficulty difficulty, ManiaPerformance performance)
        {
            return new ManiaCalculation()
            {
                Difficulty = difficulty,
                Performance = performance
            };
        }

        private CalculatorWorkingBeatmap getWorkingBeatmap(string beatmapId)
        {
            using var beatmapStream = _beatmapProvider.GetBeatmapStream(beatmapId);
            return new CalculatorWorkingBeatmap(ManiaRuleset, beatmapStream, beatmapId);
        }

        private Dictionary<HitResult, int> getHitResults(int hitResultCount, int countMiss, int countMeh, int countOk, int countGood, int countGreat)
        {
            var countPerfect = hitResultCount - (countMiss + countMeh + countOk + countGood + countGreat);

            return new Dictionary<HitResult, int>
            {
                [HitResult.Perfect] = countPerfect,
                [HitResult.Great] = countGreat,
                [HitResult.Good] = countGood,
                [HitResult.Ok] = countOk,
                [HitResult.Meh] = countMeh,
                [HitResult.Miss] = countMiss
            };
        }

        private double calculateAccuracy(Dictionary<HitResult, int> statistics)
        {
            var countPerfect = statistics[HitResult.Perfect];
            var countGreat = statistics[HitResult.Great];
            var countGood = statistics[HitResult.Good];
            var countOk = statistics[HitResult.Ok];
            var countMeh = statistics[HitResult.Meh];
            var countMiss = statistics[HitResult.Miss];
            var total = countPerfect + countGreat + countGood + countOk + countMeh + countMiss;

            return (double)((6 * countPerfect) + (6 * countGreat) + (4 * countGood) + (2 * countOk) + countMeh) / (6 * total);
        }
    }
}
