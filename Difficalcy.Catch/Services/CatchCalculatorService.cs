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
    public class CatchCalculatorService(ICache cache, IBeatmapProvider beatmapProvider) : CalculatorService<CatchScore, CatchDifficulty, CatchPerformance, CatchCalculation>(cache)
    {
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

        protected override async Task EnsureBeatmap(string beatmapId)
        {
            await beatmapProvider.EnsureBeatmap(beatmapId);
        }

        protected override (object, string) CalculateDifficultyAttributes(CatchScore score)
        {
            var workingBeatmap = GetWorkingBeatmap(score.BeatmapId);
            var mods = CatchRuleset.ConvertFromLegacyMods((LegacyMods)score.Mods).ToArray();

            var difficultyCalculator = CatchRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as CatchDifficultyAttributes;

            // Serialising anonymous object with same names because some properties can't be serialised, and the built-in JsonProperty fields aren't on all required fields
            return (difficultyAttributes, JsonSerializer.Serialize(new
            {
                difficultyAttributes.StarRating,
                difficultyAttributes.MaxCombo,
                difficultyAttributes.ApproachRate
            }));
        }

        protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<CatchDifficultyAttributes>(difficultyAttributesJson);
        }

        protected override CatchCalculation CalculatePerformance(CatchScore score, object difficultyAttributes)
        {
            var catchDifficultyAttributes = (CatchDifficultyAttributes)difficultyAttributes;

            var workingBeatmap = GetWorkingBeatmap(score.BeatmapId);
            var mods = CatchRuleset.ConvertFromLegacyMods((LegacyMods)score.Mods).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(CatchRuleset.RulesetInfo, mods);

            var combo = score.Combo ?? beatmap.HitObjects.Count(h => h is Fruit) + beatmap.HitObjects.OfType<JuiceStream>().SelectMany(j => j.NestedHitObjects).Count(h => !(h is TinyDroplet));
            var statistics = GetHitResults(beatmap, score.Misses, score.LargeDroplets, score.SmallDroplets);
            var accuracy = CalculateAccuracy(statistics);

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

        private CalculatorWorkingBeatmap GetWorkingBeatmap(string beatmapId)
        {
            using var beatmapStream = beatmapProvider.GetBeatmapStream(beatmapId);
            return new CalculatorWorkingBeatmap(CatchRuleset, beatmapStream, beatmapId);
        }

        private static Dictionary<HitResult, int> GetHitResults(IBeatmap beatmap, int countMiss, int? countDroplet, int? countTinyDroplet)
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

        private static double CalculateAccuracy(Dictionary<HitResult, int> statistics)
        {
            double hits = statistics[HitResult.Great] + statistics[HitResult.LargeTickHit] + statistics[HitResult.SmallTickHit];
            double total = hits + statistics[HitResult.Miss] + statistics[HitResult.SmallTickMiss];

            return hits / total;
        }

        private static CatchDifficulty GetDifficultyFromDifficultyAttributes(CatchDifficultyAttributes difficultyAttributes)
        {
            return new CatchDifficulty()
            {
                Total = difficultyAttributes.StarRating
            };
        }

        private static CatchPerformance GetPerformanceFromPerformanceAttributes(CatchPerformanceAttributes performanceAttributes)
        {
            return new CatchPerformance()
            {
                Total = performanceAttributes.Total
            };
        }
    }
}
