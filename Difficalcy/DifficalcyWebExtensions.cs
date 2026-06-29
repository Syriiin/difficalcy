using System.Threading.Tasks;
using Difficalcy.Services;
using Microsoft.AspNetCore.Builder;
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
}
