using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using StackExchange.Redis;

namespace Difficalcy;

public static class DifficalcyWebExtensions
{
    public static void AddDifficalcyServices(
        this WebApplicationBuilder builder,
        string openApiTitle,
        string openApiVersion
    )
    {
        builder.Logging.AddSimpleConsole(console => console.TimestampFormat = "[HH:mm:ss] ");

        var redisConfig = builder.Configuration["REDIS_CONFIGURATION"];
        ICache cache;
        if (redisConfig == null)
            cache = new InMemoryCache();
        else
            cache = new RedisCache(ConnectionMultiplexer.Connect(redisConfig));
        builder.Services.AddSingleton(cache);

        builder.Services.AddSingleton(typeof(IBeatmapProvider), typeof(WebBeatmapProvider));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(
                (document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = openApiTitle,
                        Version = openApiVersion,
                    };
                    return Task.CompletedTask;
                }
            );
        });
    }

    public static RouteGroupBuilder MapDifficalcyEndpoints<
        TScore,
        TDifficulty,
        TPerformance,
        TCalculation,
        TBeatmapDetails,
        TCalculatorService
    >(this WebApplication app)
        where TScore : Score
        where TDifficulty : Difficulty
        where TPerformance : Performance
        where TCalculation : Calculation<TDifficulty, TPerformance>
        where TBeatmapDetails : BeatmapDetails
        where TCalculatorService : CalculatorService<
                TScore,
                TDifficulty,
                TPerformance,
                TCalculation,
                TBeatmapDetails
            >
    {
        var api = app.MapGroup("/api");

        api.MapGet("/info", (TCalculatorService calculatorService) => calculatorService.Info)
            .Produces<CalculatorInfo>(StatusCodes.Status200OK);

        api.MapGet(
                "/calculation",
                async (TCalculatorService calculatorService, TScore score) =>
                {
                    try
                    {
                        return Results.Ok(await calculatorService.GetCalculation(score));
                    }
                    catch (BeatmapNotFoundException e)
                    {
                        return Results.BadRequest(new ErrorResponse(e.Message));
                    }
                }
            )
            .Produces<TCalculation>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        api.MapPost(
                "/batch/calculation",
                async (TCalculatorService calculatorService, TScore[] scores) =>
                {
                    try
                    {
                        return Results.Ok(await calculatorService.GetCalculationBatch(scores));
                    }
                    catch (BeatmapNotFoundException e)
                    {
                        return Results.BadRequest(new ErrorResponse(e.Message));
                    }
                }
            )
            .Produces<TCalculation[]>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        api.MapGet(
                "/beatmapdetails",
                async (TCalculatorService calculatorService, string beatmapId) =>
                {
                    try
                    {
                        return Results.Ok(await calculatorService.GetBeatmapDetails(beatmapId));
                    }
                    catch (BeatmapNotFoundException e)
                    {
                        return Results.BadRequest(new ErrorResponse(e.Message));
                    }
                }
            )
            .Produces<TBeatmapDetails>(StatusCodes.Status200OK)
            .Produces<ErrorResponse>(StatusCodes.Status400BadRequest);

        return api;
    }
}
