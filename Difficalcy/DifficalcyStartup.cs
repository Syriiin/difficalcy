using Difficalcy.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace Difficalcy
{
    abstract public class DifficalcyStartup(IConfiguration configuration)
    {
        public abstract string OpenApiTitle { get; }

        public abstract string OpenApiVersion { get; }

        /// <summary>
        /// The Assembly to source test beatmap resources from (eg. osu.Game.Rulesets.Osu)
        /// </summary>
        protected abstract string TestBeatmapAssembly { get; }

        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = OpenApiTitle, Version = OpenApiVersion });
            });

            var redisConfig = Configuration["REDIS_CONFIGURATION"];
            ICache cache;
            if (redisConfig == null)
                cache = new InMemoryCache();
            else
                cache = new RedisCache(ConnectionMultiplexer.Connect(redisConfig));
            services.AddSingleton<ICache>(cache);

            var useTestBeatmapProvider = Configuration["USE_TEST_BEATMAP_PROVIDER"];
            if (useTestBeatmapProvider == "true")
                services.AddSingleton<IBeatmapProvider>(new TestBeatmapProvider(TestBeatmapAssembly));
            else
                services.AddSingleton(typeof(IBeatmapProvider), typeof(WebBeatmapProvider));

            ConfigureCalculatorServices(services);
        }

        public abstract void ConfigureCalculatorServices(IServiceCollection services);

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{OpenApiTitle} {OpenApiVersion}"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
