using Difficalcy.Catch.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Difficalcy.Catch
{
    public class Startup : DifficalcyStartup
    {
        public Startup(IConfiguration configuration) : base(configuration) { }

        public override string OpenApiTitle => "Difficalcy.Catch";

        public override string OpenApiVersion => "v1";

        protected override string TestBeatmapAssembly => "osu.Game.Rulesets.Catch";

        public override void ConfigureCalculatorServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(CatchCalculatorService));
        }
    }
}
