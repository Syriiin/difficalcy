using Difficalcy;
using Difficalcy.Catch.Models;
using Difficalcy.Catch.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddDifficalcyServices("Difficalcy.Catch", "v1");

builder.Services.AddSingleton<CatchCalculatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapOpenApi();

app.MapDifficalcyEndpoints<
    CatchScore,
    CatchDifficulty,
    CatchPerformance,
    CatchCalculation,
    CatchBeatmapDetails,
    CatchCalculatorService
>();

app.Run();
