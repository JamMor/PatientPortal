using System;
using System.IO;
using System.Threading.Tasks; // TODO: MIGRATION
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity; // TODO: MIGRATION
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PatientPortal.Configuration;
using PatientPortal.Models;
using PatientPortal.Services; // TODO: MIGRATION - Remove after staff data migration

// Load environment variables from .env file
string rootDir = Directory.GetCurrentDirectory();
string dotenvPath = Path.Combine(rootDir, ".env");
DotEnv.Load(dotenvPath);

var builder = WebApplication.CreateBuilder(args);

// TODO: MIGRATION START - Remove this entire block after staff data migration
// Check for migration command before building web app
if (args.Length > 0 && args[0] == "migrate-staff")
{
    await RunStaffMigration(builder);
    return;
}
// TODO: MIGRATION END

// Add database context
var connectionString = builder.Configuration["DBInfo:ConnectionString"];
builder.Services.AddDbContext<PatientPortalContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23)))
);

// Add other services
builder.Services.AddHttpContextAccessor();
// TODO: Deprecated. Pending removal.
// builder.Services.AddSession();
builder.Services.AddIdentityServices();
builder.Services.AddCoreServices();
builder.Services.AddViewServices();
builder.Services.AddTestServices();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

// TODO: Deprecated. Pending removal.
// app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// TODO: MIGRATION START - Remove this entire method after staff data migration
static async Task RunStaffMigration(WebApplicationBuilder builder)
{
    Console.WriteLine("Entering staff data migration mode...");
    
    // Build services needed for migration
    var connectionString = builder.Configuration["DBInfo:ConnectionString"];
    builder.Services.AddDbContext<PatientPortalContext>(options =>
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23)))
    );
    builder.Services.AddIdentityServices();
    
    var app = builder.Build();
    
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PatientPortalContext>();
    
    // Run database migrations first
    Console.WriteLine("Applying database migrations...");
    await context.Database.MigrateAsync();
    Console.WriteLine("Database migrations completed.");
    
    // Then migrate staff data
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var migration = new StaffPasswordMigration(userManager, context);
    
    Console.WriteLine("Starting staff data migration...");
    await migration.MigrateAsync();
    Console.WriteLine("Staff data migration completed.");
}
// TODO: MIGRATION END
