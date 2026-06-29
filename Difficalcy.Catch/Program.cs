using Difficalcy;
using Difficalcy.Catch.Models;
using Difficalcy.Catch.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddDifficalcyServices("Difficalcy.Catch", "v1");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, CatchJsonContext.Default);
});

builder.Services.AddSingleton<CatchCalculatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapOpenApi();

var handlers = new DifficalcyHandlers<
    CatchScore,
    CatchDifficulty,
    CatchPerformance,
    CatchCalculation,
    CatchBeatmapDetails
>(app.Services.GetRequiredService<CatchCalculatorService>());

var api = app.MapGroup("/api");

api.MapGet("/info", handlers.GetInfo);
api.MapGet("/calculation", handlers.GetCalculation);
api.MapPost("/batch/calculation", handlers.GetCalculationBatch);
api.MapGet("/beatmapdetails", handlers.GetBeatmapDetails);

app.Run();
