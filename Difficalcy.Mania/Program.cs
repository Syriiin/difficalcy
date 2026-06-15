using Difficalcy;
using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddDifficalcyServices("Difficalcy.Mania", "v1");

builder.Services.AddSingleton<ManiaCalculatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapOpenApi();

app.MapDifficalcyEndpoints<
    ManiaScore,
    ManiaDifficulty,
    ManiaPerformance,
    ManiaCalculation,
    ManiaBeatmapDetails,
    ManiaCalculatorService
>();

app.Run();
