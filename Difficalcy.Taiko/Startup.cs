using System.Reflection;
using Difficalcy.Taiko.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Difficalcy.Taiko
{
    public class Startup(IConfiguration configuration) : DifficalcyStartup(configuration)
    {
        public override string OpenApiTitle => "Difficalcy.Taiko";

        public override string OpenApiVersion => "v1";

        protected override string TestBeatmapAssembly => Assembly.GetExecutingAssembly().GetName().Name;

        public override void ConfigureCalculatorServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(TaikoCalculatorService));
        }
    }
}
