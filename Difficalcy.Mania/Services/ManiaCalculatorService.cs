using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Mania.Models;
using Difficalcy.Models;
using Difficalcy.Services;
using Microsoft.Extensions.Configuration;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Mania.Difficulty;
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
            var mods = ManiaRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();

            var difficultyCalculator = ManiaRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as ManiaDifficultyAttributes;

            // Serialising anonymous object with same names because Mods and Skills can't be serialised
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
            return JsonSerializer.Deserialize<ManiaDifficultyAttributes>(difficultyAttributesJson, new JsonSerializerOptions() { IncludeFields = true });
        }

        protected override ManiaPerformance CalculatePerformance(ManiaScore score, object difficultyAttributes)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = ManiaRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(ManiaRuleset.RulesetInfo, mods);

            var hitResultCount = beatmap.HitObjects.Count;
            var statistics = new Dictionary<HitResult, int>
            {
                { HitResult.Perfect, hitResultCount },
                { HitResult.Great, 0 },
                { HitResult.Ok, 0 },
                { HitResult.Good, 0 },
                { HitResult.Meh, 0 },
                { HitResult.Miss, 0 }
            };
            var totalScore = score.TotalScore ?? determineScore(mods);

            var scoreInfo = new ScoreInfo()
            {
                Accuracy = 0,
                MaxCombo = 0,
                Statistics = statistics,
                Mods = mods,
                TotalScore = totalScore
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

        private int determineScore(Mod[] mods)
        {
            double scoreMultiplier = 1;

            foreach (var mod in mods)
            {
                if (mod.Type == ModType.DifficultyReduction)
                    scoreMultiplier *= mod.ScoreMultiplier;
            }

            return (int)Math.Round(1000000 * scoreMultiplier);
        }
    }
}
