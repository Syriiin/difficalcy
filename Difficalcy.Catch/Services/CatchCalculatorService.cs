using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Catch.Models;
using Difficalcy.Models;
using Difficalcy.Services;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient = new HttpClient();
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

        public CatchCalculatorService(IConfiguration configuration, ICache cache) : base(cache)
        {
            _configuration = configuration;
        }

        public override async Task EnsureBeatmap(int beatmapId)
        {
            var beatmapPath = Path.Combine(_configuration["BEATMAP_DIRECTORY"], beatmapId.ToString());
            if (!File.Exists(beatmapPath))
            {
                var response = await _httpClient.GetStreamAsync($"https://osu.ppy.sh/osu/{beatmapId}");
                using (var fs = new FileStream(beatmapPath, FileMode.CreateNew))
                    await response.CopyToAsync(fs);
            }
        }

        public override (object, string) CalculateDifficulty(CatchScore score)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = CatchRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();

            var difficultyCalculator = CatchRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as CatchDifficultyAttributes;

            // Serialising anonymous object with same names because Mods and Skills can't be serialised
            return (difficultyAttributes, JsonSerializer.Serialize(new
            {
                StarRating = difficultyAttributes.StarRating,
                MaxCombo = difficultyAttributes.MaxCombo,
                ApproachRate = difficultyAttributes.ApproachRate
            }));
        }

        public override CatchDifficulty GetDifficultyFromDifficultyAttributes(object difficultyAttributes)
        {
            var catchDifficultyAttributes = (CatchDifficultyAttributes)difficultyAttributes;
            return new CatchDifficulty()
            {
                Total = catchDifficultyAttributes.StarRating
            };
        }

        public override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<CatchDifficultyAttributes>(difficultyAttributesJson, new JsonSerializerOptions() { IncludeFields = true });
        }

        public override CatchPerformance CalculatePerformance(CatchScore score, object difficultyAttributes)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = CatchRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(CatchRuleset.RulesetInfo, mods);

            var hitResultCount = beatmap.HitObjects.Count(h => h is Fruit) + beatmap.HitObjects.OfType<JuiceStream>().SelectMany(j => j.NestedHitObjects).Count(h => !(h is TinyDroplet));
            var combo = score.Combo ?? hitResultCount;
            var statistics = determineHitResults(score.Accuracy ?? 1, hitResultCount, beatmap, score.Misses ?? 0, score.TinyDroplets, score.Droplets);
            var accuracy = score.Accuracy ?? calculateAccuracy(statistics);

            var scoreInfo = new ScoreInfo()
            {
                Accuracy = accuracy,
                MaxCombo = combo,
                Statistics = statistics,
                Mods = mods
            };

            var performanceCalculator = CatchRuleset.CreatePerformanceCalculator((CatchDifficultyAttributes)difficultyAttributes, scoreInfo);
            var categoryAttributes = new Dictionary<string, double>();
            var performance = performanceCalculator.Calculate(categoryAttributes);

            return new CatchPerformance()
            {
                Total = performance
            };
        }

        public override CatchCalculation GetCalculation(CatchDifficulty difficulty, CatchPerformance performance)
        {
            return new CatchCalculation()
            {
                Difficulty = difficulty,
                Performance = performance
            };
        }

        private CalculatorWorkingBeatmap getWorkingBeatmap(int beatmapId)
        {
            var beatmapPath = Path.Combine(_configuration["BEATMAP_DIRECTORY"], beatmapId.ToString());
            return new CalculatorWorkingBeatmap(CatchRuleset, beatmapPath, beatmapId);
        }

        private Dictionary<HitResult, int> determineHitResults(double targetAccuracy, int hitResultCount, IBeatmap beatmap, int countMiss, int? countTinyDroplets, int? countDroplet)
        {
            // Adapted from https://github.com/ppy/osu-tools/blob/cf5410b04f4e2d1ed2c50c7263f98c8fc5f928ab/PerformanceCalculator/Simulate/CatchSimulateCommand.cs#L58-L86
            int maxTinyDroplets = beatmap.HitObjects.OfType<JuiceStream>().Sum(s => s.NestedHitObjects.OfType<TinyDroplet>().Count());
            int maxDroplets = beatmap.HitObjects.OfType<JuiceStream>().Sum(s => s.NestedHitObjects.OfType<Droplet>().Count()) - maxTinyDroplets;
            int maxFruits = beatmap.HitObjects.OfType<Fruit>().Count() + 2 * beatmap.HitObjects.OfType<JuiceStream>().Count() + beatmap.HitObjects.OfType<JuiceStream>().Sum(s => s.RepeatCount);

            // Either given or max value minus misses
            int countDroplets = countDroplet ?? Math.Max(0, maxDroplets - countMiss);

            // Max value minus whatever misses are left. Negative if impossible missCount
            int countFruits = maxFruits - (countMiss - (maxDroplets - countDroplets));

            // Either given or the max amount of hit objects with respect to accuracy minus the already calculated fruits and drops.
            // Negative if accuracy not feasable with missCount.
            int countTinyDroplet = countTinyDroplets ?? (int)Math.Round(targetAccuracy * (hitResultCount + maxTinyDroplets)) - countFruits - countDroplets;

            // Whatever droplets are left
            int countTinyMisses = maxTinyDroplets - countTinyDroplet;

            return new Dictionary<HitResult, int>
            {
                { HitResult.Great, countFruits },
                { HitResult.LargeTickHit, countDroplets },
                { HitResult.SmallTickHit, countTinyDroplet },
                { HitResult.SmallTickMiss, countTinyMisses },
                { HitResult.Miss, countMiss }
            };
        }

        private double calculateAccuracy(Dictionary<HitResult, int> statistics)
        {
            double hits = statistics[HitResult.Great] + statistics[HitResult.LargeTickHit] + statistics[HitResult.SmallTickHit];
            double total = hits + statistics[HitResult.Miss] + statistics[HitResult.SmallTickMiss];

            return hits / total;
        }
    }
}
