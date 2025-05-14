using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PatientPortal.Configuration;
using PatientPortal.Models;

// Load environment variables from .env file
string rootDir = Directory.GetCurrentDirectory();
string dotenvPath = Path.Combine(rootDir, ".env");
DotEnv.Load(dotenvPath);

var builder = WebApplication.CreateBuilder(args);

// Add database context
var connectionString = builder.Configuration["DBInfo:ConnectionString"];
builder.Services.AddDbContext<PatientPortalContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23)))
);

// Add other services
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
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

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
