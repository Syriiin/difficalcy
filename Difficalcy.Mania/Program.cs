using Difficalcy;
using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddDifficalcyServices("Difficalcy.Mania", "v1");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, ManiaJsonContext.Default);
});

builder.Services.AddSingleton<ManiaCalculatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapOpenApi();

var handlers = new DifficalcyHandlers<
    ManiaScore,
    ManiaDifficulty,
    ManiaPerformance,
    ManiaCalculation,
    ManiaBeatmapDetails
>(app.Services.GetRequiredService<ManiaCalculatorService>());

var api = app.MapGroup("/api");

api.MapGet("/info", handlers.GetInfo);
api.MapGet("/calculation", handlers.GetCalculation);
api.MapPost("/batch/calculation", handlers.GetCalculationBatch);
api.MapGet("/beatmapdetails", handlers.GetBeatmapDetails);

app.Run();
