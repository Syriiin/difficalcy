using System.Reflection;
using Difficalcy.Mania.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Difficalcy.Mania
{
    public class Startup(IConfiguration configuration) : DifficalcyStartup(configuration)
    {
        public override string OpenApiTitle => "Difficalcy.Mania";

        public override string OpenApiVersion => "v1";

        protected override string TestBeatmapAssembly => Assembly.GetExecutingAssembly().GetName().Name;

        public override void ConfigureCalculatorServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ManiaCalculatorService));
        }
    }
}
