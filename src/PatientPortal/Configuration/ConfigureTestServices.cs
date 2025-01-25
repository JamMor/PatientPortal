using Microsoft.Extensions.DependencyInjection;
using PatientPortal.Interfaces;
using PatientPortal.Services;

namespace PatientPortal.Configuration
{
    public static class ConfigureTestServices
    {
        public static IServiceCollection AddTestServices(this IServiceCollection services)
        {
            services.AddTransient<ITestLoginService, TestLoginService>();
            services.AddTransient<ISeedService, SeedService>();
            services.AddTransient<ISeedViewService, SeedViewService>();

            return services;
        }
    }
}