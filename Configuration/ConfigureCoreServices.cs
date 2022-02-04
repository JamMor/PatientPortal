using Microsoft.Extensions.DependencyInjection;
using PatientPortal.Interfaces;
using PatientPortal.Services;

namespace PatientPortal.Configuration
{
    public static class ConfigureCoreServices
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<ITestLoginService, TestLoginService>();
            services.AddTransient<ISeedService, SeedService>();
            services.AddTransient<IStaffService, StaffService>();
            services.AddTransient<IPatientService, PatientService>();
            services.AddTransient<IPatientStaffConnectionService, PatientStaffConnectionService>();
            services.AddTransient<IHealthIssueService, HealthIssueService>();
            services.AddTransient<IVisitService, VisitService>();
            services.AddTransient<ITestResultService, TestResultService>();
            services.AddTransient<IMessagingService, MessagingService>();

            return services;
        }
    }
}