using Microsoft.Extensions.DependencyInjection;
using PatientPortal.Interfaces;
using PatientPortal.Services;

namespace PatientPortal.Configuration
{
    public static class ConfigureViewServices
    {
        public static IServiceCollection AddViewServices(this IServiceCollection services)
        {
            services.AddTransient<ISeedViewService, SeedViewService>();
            services.AddTransient<IStaffViewService, StaffViewService>();
            services.AddTransient<IPatientViewService, PatientViewService>();
            services.AddTransient<IMessagingViewService, MessagingViewService>();

            return services;
        }
    }
}