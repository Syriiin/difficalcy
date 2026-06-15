using Difficalcy;
using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.AddDifficalcyServices("Difficalcy.Taiko", "v1");

builder.Services.AddSingleton<TaikoCalculatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

app.MapOpenApi();

app.MapDifficalcyEndpoints<
    TaikoScore,
    TaikoDifficulty,
    TaikoPerformance,
    TaikoCalculation,
    TaikoBeatmapDetails,
    TaikoCalculatorService
>();

app.Run();
