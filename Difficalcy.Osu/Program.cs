using Difficalcy;
using Difficalcy.Osu.Models;
using Difficalcy.Osu.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddDifficalcyServices("Difficalcy.Osu", "v1");

builder.Services.AddSingleton<OsuCalculatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapOpenApi();

app.MapDifficalcyEndpoints<
    OsuScore,
    OsuDifficulty,
    OsuPerformance,
    OsuCalculation,
    OsuBeatmapDetails,
    OsuCalculatorService
>();

app.Run();
