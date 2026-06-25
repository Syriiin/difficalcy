using Difficalcy;
using Difficalcy.Osu.Models;
using Difficalcy.Osu.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddDifficalcyServices("Difficalcy.Osu", "v1");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, OsuJsonContext.Default);
});

builder.Services.AddSingleton<OsuCalculatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapOpenApi();

var handlers = new DifficalcyHandlers<
    OsuScore,
    OsuDifficulty,
    OsuPerformance,
    OsuCalculation,
    OsuBeatmapDetails
>(app.Services.GetRequiredService<OsuCalculatorService>());

var api = app.MapGroup("/api");

api.MapGet("/info", handlers.GetInfo);
api.MapGet("/calculation", handlers.GetCalculation);
api.MapPost("/batch/calculation", handlers.GetCalculationBatch);
api.MapGet("/beatmapdetails", handlers.GetBeatmapDetails);

app.Run();
