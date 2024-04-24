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
            var mods = ManiaRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();

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
            var mods = ManiaRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(ManiaRuleset.RulesetInfo, mods);

            var hitResultCount = beatmap.HitObjects.Count;
            var holdNoteCount = beatmap.HitObjects.OfType<HoldNote>().Count();
            var statistics = determineHitResults(score.Accuracy ?? 1, hitResultCount, holdNoteCount, score.Misses ?? 0, score.Mehs, score.Oks, score.Goods, score.Greats);
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

        private Dictionary<HitResult, int> determineHitResults(double targetAccuracy, int hitObjectCount, int holdNoteCount, int countMiss, int? countMeh, int? countOk, int? countGood, int? countGreat)
        {
            // Adapted from https://github.com/ppy/osu-tools/blob/c3cbe410bb1255fc5884a3daf2df3e8f87a0a8a8/PerformanceCalculator/Simulate/ManiaSimulateCommand.cs#L54-L109
            // One judgement per normal note. Two judgements per hold note (head + tail).
            var totalHits = hitObjectCount + holdNoteCount;

            if (countMeh != null || countOk != null || countGood != null || countGreat != null)
            {
                int countPerfect = totalHits - (countMiss + (countMeh ?? 0) + (countOk ?? 0) + (countGood ?? 0) + (countGreat ?? 0));

                return new Dictionary<HitResult, int>
                {
                    [HitResult.Perfect] = countPerfect,
                    [HitResult.Great] = countGreat ?? 0,
                    [HitResult.Good] = countGood ?? 0,
                    [HitResult.Ok] = countOk ?? 0,
                    [HitResult.Meh] = countMeh ?? 0,
                    [HitResult.Miss] = countMiss
                };
            }

            // Let Perfect=Great=6, Good=4, Ok=2, Meh=1, Miss=0. The total should be this.
            var targetTotal = (int)Math.Round(targetAccuracy * totalHits * 6);

            // Start by assuming every non miss is a meh
            // This is how much increase is needed by the rest
            int tempMeh = totalHits - countMiss;
            int remainingToFill = targetTotal - tempMeh;

            // Each great and perfect increases total by 5 (great-meh=5)
            // There is no difference in accuracy between them, so just call them perfects.
            int perfects = Math.Min(remainingToFill / 5, tempMeh);
            remainingToFill -= perfects * 5;
            tempMeh -= perfects;

            // Each good increases total by 3 (good-meh=3).
            int goods = Math.Min(remainingToFill / 3, tempMeh);
            remainingToFill -= goods * 3;
            tempMeh -= goods;

            // Each ok increases total by 1 (ok-meh=1).
            int oks = remainingToFill;
            tempMeh -= oks;

            // Everything else is a meh, as initially assumed.
            int mehs = tempMeh;

            return new Dictionary<HitResult, int>
            {
                { HitResult.Perfect, perfects },
                { HitResult.Great, 0 },
                { HitResult.Good, goods },
                { HitResult.Ok, oks },
                { HitResult.Meh, mehs },
                { HitResult.Miss, countMiss }
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
