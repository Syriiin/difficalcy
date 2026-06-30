using System.Collections.Generic;
using Difficalcy;
using Difficalcy.Catch.Models;
using Difficalcy.Catch.Services;
using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;
using Difficalcy.Models;
using Difficalcy.Osu.Models;
using Difficalcy.Osu.Services;
using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddDifficalcyServices("Difficalcy", "v1");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Add(DifficalcyJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(OsuJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(TaikoJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(CatchJsonContext.Default);
    options.SerializerOptions.TypeInfoResolverChain.Add(ManiaJsonContext.Default);
});

builder.Services.AddSingleton<OsuCalculatorService>();
builder.Services.AddSingleton<TaikoCalculatorService>();
builder.Services.AddSingleton<CatchCalculatorService>();
builder.Services.AddSingleton<ManiaCalculatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapOpenApi();

var api = app.MapGroup("/api");

api.MapGet(
    "/calculators",
    () =>
    {
        var osu = app.Services.GetRequiredService<OsuCalculatorService>();
        var taiko = app.Services.GetRequiredService<TaikoCalculatorService>();
        var catch_ = app.Services.GetRequiredService<CatchCalculatorService>();
        var mania = app.Services.GetRequiredService<ManiaCalculatorService>();

        return new Dictionary<string, CalculatorInfo>
        {
            ["osu"] = osu.Info,
            ["taiko"] = taiko.Info,
            ["catch"] = catch_.Info,
            ["mania"] = mania.Info,
        };
    }
);

var osuHandlers = new DifficalcyHandlers<
    OsuScore,
    OsuDifficulty,
    OsuPerformance,
    OsuCalculation,
    OsuBeatmapDetails
>(app.Services.GetRequiredService<OsuCalculatorService>());
var osu = api.MapGroup("/calculators/osu");
osu.MapGet("/info", osuHandlers.GetInfo);
osu.MapGet("/calculation", osuHandlers.GetCalculation);
osu.MapPost("/batch/calculation", osuHandlers.GetCalculationBatch);
osu.MapGet("/beatmapdetails", osuHandlers.GetBeatmapDetails);

var taikoHandlers = new DifficalcyHandlers<
    TaikoScore,
    TaikoDifficulty,
    TaikoPerformance,
    TaikoCalculation,
    TaikoBeatmapDetails
>(app.Services.GetRequiredService<TaikoCalculatorService>());
var taiko = api.MapGroup("/calculators/taiko");
taiko.MapGet("/info", taikoHandlers.GetInfo);
taiko.MapGet("/calculation", taikoHandlers.GetCalculation);
taiko.MapPost("/batch/calculation", taikoHandlers.GetCalculationBatch);
taiko.MapGet("/beatmapdetails", taikoHandlers.GetBeatmapDetails);

var catchHandlers = new DifficalcyHandlers<
    CatchScore,
    CatchDifficulty,
    CatchPerformance,
    CatchCalculation,
    CatchBeatmapDetails
>(app.Services.GetRequiredService<CatchCalculatorService>());
var catch_ = api.MapGroup("/calculators/catch");
catch_.MapGet("/info", catchHandlers.GetInfo);
catch_.MapGet("/calculation", catchHandlers.GetCalculation);
catch_.MapPost("/batch/calculation", catchHandlers.GetCalculationBatch);
catch_.MapGet("/beatmapdetails", catchHandlers.GetBeatmapDetails);

var maniaHandlers = new DifficalcyHandlers<
    ManiaScore,
    ManiaDifficulty,
    ManiaPerformance,
    ManiaCalculation,
    ManiaBeatmapDetails
>(app.Services.GetRequiredService<ManiaCalculatorService>());
var mania = api.MapGroup("/calculators/mania");
mania.MapGet("/info", maniaHandlers.GetInfo);
mania.MapGet("/calculation", maniaHandlers.GetCalculation);
mania.MapPost("/batch/calculation", maniaHandlers.GetCalculationBatch);
mania.MapGet("/beatmapdetails", maniaHandlers.GetBeatmapDetails);

app.Run();
