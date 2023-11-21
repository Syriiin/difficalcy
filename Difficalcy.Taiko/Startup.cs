using Difficalcy.Taiko.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Difficalcy.Taiko
{
    public class Startup : DifficalcyStartup
    {
        public Startup(IConfiguration configuration) : base(configuration) { }

        public override string OpenApiTitle => "Difficalcy.Taiko";

        public override string OpenApiVersion => "v1";

        protected override string TestBeatmapAssembly => "osu.Game.Rulesets.Taiko";

        public override void ConfigureCalculatorServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(TaikoCalculatorService));
        }
    }
}
