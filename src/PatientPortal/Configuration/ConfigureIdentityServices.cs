
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PatientPortal.Authorization.Handlers;
using PatientPortal.Authorization.Requirements;
using PatientPortal.Infrastructure;
using PatientPortal.Models;

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
                // TODO: Current usernames use special characters, but consider restricting in future
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+<>$";
                
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<PatientPortalContext>()
            .AddDefaultTokenProviders();

            // Register custom claims principal factory
            services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, CustomUserClaimsPrincipalFactory>();

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

            // Configure authorization policies
            services.AddAuthorization(options =>
            {
                // Set fallback policy to require authenticated users by default
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(Authorization.PolicyNames.ManageStaff, policy =>
                    policy.Requirements.Add(new AdminRequirement()));

                options.AddPolicy(Authorization.PolicyNames.ManagePatients, policy =>
                    policy.Requirements.Add(new StaffMemberRequirement()));

                options.AddPolicy(Authorization.PolicyNames.MessagePatients, policy =>
                    policy.Requirements.Add(new StaffMemberRequirement()));
            });

            // Register authorization handlers
            services.AddSingleton<IAuthorizationHandler, AdminRequirementHandler>();
            services.AddSingleton<IAuthorizationHandler, StaffMemberRequirementHandler>();

            return services;
        }
    }
}