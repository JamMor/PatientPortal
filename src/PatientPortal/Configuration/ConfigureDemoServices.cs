using Microsoft.Extensions.DependencyInjection;
using PatientPortal.Interfaces;
using PatientPortal.Services;

namespace PatientPortal.Configuration
{
    public static class ConfigureDemoServices
    {
        public static IServiceCollection AddDemoServices(this IServiceCollection services)
        {
            services.AddTransient<IDemoLoginService, DemoLoginService>();

            return services;
        }
    }
}