using System.Text.Json.Serialization;
using Difficalcy.Models;
using osu.Game.Rulesets.Catch.Difficulty;

namespace Difficalcy.Catch.Models;

// Response models
[JsonSerializable(typeof(CalculatorInfo))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(CatchCalculation))]
[JsonSerializable(typeof(CatchCalculation[]))]
[JsonSerializable(typeof(CatchBeatmapDetails))]
// Request models
[JsonSerializable(typeof(Mod), TypeInfoPropertyName = "ScoreMod")]
[JsonSerializable(typeof(Mod[]), TypeInfoPropertyName = "ScoreModArray")]
[JsonSerializable(typeof(CatchScore))]
[JsonSerializable(typeof(CatchScore[]))]
// Internal models
[JsonSerializable(typeof(CatchDifficultyAttributes))]
[JsonSerializable(typeof(CatchDifficultyAttributesDto))]
public partial class CatchJsonContext : JsonSerializerContext { }
