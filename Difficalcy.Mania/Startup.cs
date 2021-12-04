using Difficalcy.Mania.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Difficalcy.Mania
{
    public class Startup : DifficalcyStartup
    {
        public Startup(IConfiguration configuration) : base(configuration) { }

        public override string OpenApiTitle => "Difficalcy.Mania";

        public override string OpenApiVersion => "v1";

        public override void ConfigureCalculatorServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(ManiaCalculatorService));
        }
    }
}
