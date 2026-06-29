using System.Linq;
using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Difficalcy;

public class DifficalcyHandlers<TScore, TDifficulty, TPerformance, TCalculation, TBeatmapDetails>(
    CalculatorService<
        TScore,
        TDifficulty,
        TPerformance,
        TCalculation,
        TBeatmapDetails
    > calculatorService
)
    where TScore : Score, new()
    where TDifficulty : Difficulty
    where TPerformance : Performance
    where TCalculation : Calculation<TDifficulty, TPerformance>
    where TBeatmapDetails : BeatmapDetails
{
    public CalculatorInfo GetInfo() => calculatorService.Info;

    public async Task<Results<Ok<TCalculation>, BadRequest<ErrorResponse>>> GetCalculation(
        string beatmapId,
        string mods = ""
    )
    {
        var score = new TScore
        {
            BeatmapId = beatmapId,
            Mods = [.. mods.Split(',').Select(acronym => new Mod { Acronym = acronym })],
        };
        try
        {
            return TypedResults.Ok(await calculatorService.GetCalculation(score));
        }
        catch (BeatmapNotFoundException e)
        {
            return TypedResults.BadRequest(new ErrorResponse(e.Message));
        }
    }

    public async Task<Results<Ok<TCalculation[]>, BadRequest<ErrorResponse>>> GetCalculationBatch(
        TScore[] scores
    )
    {
        try
        {
            return TypedResults.Ok((await calculatorService.GetCalculationBatch(scores)).ToArray());
        }
        catch (BeatmapNotFoundException e)
        {
            return TypedResults.BadRequest(new ErrorResponse(e.Message));
        }
    }

    public async Task<Results<Ok<TBeatmapDetails>, BadRequest<ErrorResponse>>> GetBeatmapDetails(
        string beatmapId
    )
    {
        try
        {
            return TypedResults.Ok(await calculatorService.GetBeatmapDetails(beatmapId));
        }
        catch (BeatmapNotFoundException e)
        {
            return TypedResults.BadRequest(new ErrorResponse(e.Message));
        }
    }
}
