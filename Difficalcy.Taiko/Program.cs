using Difficalcy;
using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddDifficalcyServices("Difficalcy.Taiko", "v1");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, TaikoJsonContext.Default);
});

builder.Services.AddSingleton<TaikoCalculatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapOpenApi();

var handlers = new DifficalcyHandlers<
    TaikoScore,
    TaikoDifficulty,
    TaikoPerformance,
    TaikoCalculation,
    TaikoBeatmapDetails
>(app.Services.GetRequiredService<TaikoCalculatorService>());

var api = app.MapGroup("/api");

api.MapGet("/info", handlers.GetInfo);
api.MapGet("/calculation", handlers.GetCalculation);
api.MapPost("/batch/calculation", handlers.GetCalculationBatch);
api.MapGet("/beatmapdetails", handlers.GetBeatmapDetails);

app.Run();
