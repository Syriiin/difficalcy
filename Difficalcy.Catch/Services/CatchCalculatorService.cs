using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Catch.Models;
using Difficalcy.Models;
using Difficalcy.Services;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Catch.Difficulty;
using osu.Game.Rulesets.Catch.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace Difficalcy.Catch.Services
{
    public class CatchCalculatorService : CalculatorService<CatchScore, CatchDifficulty, CatchPerformance, CatchCalculation>
    {
        private readonly IBeatmapProvider _beatmapProvider;
        private CatchRuleset CatchRuleset { get; } = new CatchRuleset();

        public override CalculatorInfo Info
        {
            get
            {
                var packageName = Assembly.GetAssembly(typeof(CatchRuleset)).GetName().Name;
                var packageVersion = Assembly.GetAssembly(typeof(CatchRuleset)).GetName().Version.ToString();
                return new CalculatorInfo
                {
                    RulesetName = CatchRuleset.Description,
                    CalculatorName = "Official osu!catch",
                    CalculatorPackage = packageName,
                    CalculatorVersion = packageVersion,
                    CalculatorUrl = $"https://nuget.org/packages/ppy.{packageName}/{packageVersion}"
                };
            }
        }

        public CatchCalculatorService(ICache cache, IBeatmapProvider beatmapProvider) : base(cache)
        {
            _beatmapProvider = beatmapProvider;
        }

        protected override async Task EnsureBeatmap(string beatmapId)
        {
            await _beatmapProvider.EnsureBeatmap(beatmapId);
        }

        protected override (object, string) CalculateDifficultyAttributes(CatchScore score)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = CatchRuleset.ConvertFromLegacyMods((LegacyMods)score.Mods).ToArray();

            var difficultyCalculator = CatchRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as CatchDifficultyAttributes;

            // Serialising anonymous object with same names because some properties can't be serialised, and the built-in JsonProperty fields aren't on all required fields
            return (difficultyAttributes, JsonSerializer.Serialize(new
            {
                StarRating = difficultyAttributes.StarRating,
                MaxCombo = difficultyAttributes.MaxCombo,
                ApproachRate = difficultyAttributes.ApproachRate
            }));
        }

        protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<CatchDifficultyAttributes>(difficultyAttributesJson);
        }

        protected override CatchCalculation CalculatePerformance(CatchScore score, object difficultyAttributes)
        {
            var catchDifficultyAttributes = (CatchDifficultyAttributes)difficultyAttributes;

            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = CatchRuleset.ConvertFromLegacyMods((LegacyMods)score.Mods).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(CatchRuleset.RulesetInfo, mods);

            var combo = score.Combo ?? beatmap.HitObjects.Count(h => h is Fruit) + beatmap.HitObjects.OfType<JuiceStream>().SelectMany(j => j.NestedHitObjects).Count(h => !(h is TinyDroplet));
            var statistics = getHitResults(beatmap, score.Misses, score.LargeDroplets, score.SmallDroplets);
            var accuracy = calculateAccuracy(statistics);

            var scoreInfo = new ScoreInfo(beatmap.BeatmapInfo, CatchRuleset.RulesetInfo)
            {
                Accuracy = accuracy,
                MaxCombo = combo,
                Statistics = statistics,
                Mods = mods
            };

            var performanceCalculator = CatchRuleset.CreatePerformanceCalculator();
            var performanceAttributes = performanceCalculator.Calculate(scoreInfo, catchDifficultyAttributes) as CatchPerformanceAttributes;

            return new CatchCalculation()
            {
                Difficulty = GetDifficultyFromDifficultyAttributes(catchDifficultyAttributes),
                Performance = GetPerformanceFromPerformanceAttributes(performanceAttributes),
                Accuracy = accuracy,
                Combo = combo
            };
        }

        private CalculatorWorkingBeatmap getWorkingBeatmap(string beatmapId)
        {
            using var beatmapStream = _beatmapProvider.GetBeatmapStream(beatmapId);
            return new CalculatorWorkingBeatmap(CatchRuleset, beatmapStream, beatmapId);
        }

        private Dictionary<HitResult, int> getHitResults(IBeatmap beatmap, int countMiss, int? countDroplet, int? countTinyDroplet)
        {
            var maxTinyDroplets = beatmap.HitObjects.OfType<JuiceStream>().Sum(s => s.NestedHitObjects.OfType<TinyDroplet>().Count());
            var maxDroplets = beatmap.HitObjects.OfType<JuiceStream>().Sum(s => s.NestedHitObjects.OfType<Droplet>().Count()) - maxTinyDroplets;
            var maxFruits = beatmap.HitObjects.OfType<Fruit>().Count() + 2 * beatmap.HitObjects.OfType<JuiceStream>().Count() + beatmap.HitObjects.OfType<JuiceStream>().Sum(s => s.RepeatCount);

            var countDroplets = countDroplet ?? maxDroplets;
            var countTinyDroplets = countTinyDroplet ?? maxTinyDroplets;

            var countDropletMiss = maxDroplets - countDroplets;
            var fruitMisses = countMiss - countDropletMiss;
            var countFruit = maxFruits - fruitMisses;

            var countTinyDropletMiss = maxTinyDroplets - countTinyDroplets;

            return new Dictionary<HitResult, int>
            {
                { HitResult.Great, countFruit },
                { HitResult.LargeTickHit, countDroplets },
                { HitResult.SmallTickHit, countTinyDroplets },
                { HitResult.Miss, countMiss }, // fruit + large droplet misses
                // { HitResult.LargeTickMiss, countDropletMiss }, // included in misses for legacy compatibility
                { HitResult.SmallTickMiss, countTinyDropletMiss },
            };
        }

        private double calculateAccuracy(Dictionary<HitResult, int> statistics)
        {
            double hits = statistics[HitResult.Great] + statistics[HitResult.LargeTickHit] + statistics[HitResult.SmallTickHit];
            double total = hits + statistics[HitResult.Miss] + statistics[HitResult.SmallTickMiss];

            return hits / total;
        }

        private CatchDifficulty GetDifficultyFromDifficultyAttributes(CatchDifficultyAttributes difficultyAttributes)
        {
            return new CatchDifficulty()
            {
                Total = difficultyAttributes.StarRating
            };
        }

        private CatchPerformance GetPerformanceFromPerformanceAttributes(CatchPerformanceAttributes performanceAttributes)
        {
            return new CatchPerformance()
            {
                Total = performanceAttributes.Total
            };
        }
    }
}
