
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using PatientPortal.Models;
using System;

namespace PatientPortal.Configuration
{
    public static class ConfigureIdentityServices
    {
        public static IServiceCollection AddIdentityServices( this IServiceCollection services)
        {
            // Add Identity services
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequiredLength = 10;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                
                // User settings
                options.User.RequireUniqueEmail = false;
                
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<PatientPortalContext>()
            .AddDefaultTokenProviders();

            // Configure application cookie
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            });
            return services;
        }
    }
}