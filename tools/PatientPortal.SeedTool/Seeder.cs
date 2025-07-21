using PatientPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace PatientPortal.SeedTool
{
    public static class Seeder
    {
        public static async Task SeedDatabase(int staffCount, int patientCount, string connectionString)
        {;
            // TODO: Implement actual seeding logic here
            Console.WriteLine("=== PatientPortal Seed Tool ===");
            Console.WriteLine($"Staff to create: {staffCount}");
            Console.WriteLine($"Patients to create: {patientCount}");
            Console.WriteLine($"Connection string provided: {connectionString}");
            Console.WriteLine();
            Console.WriteLine("================================================");

            try
            {
                // Create database context
                var optionsBuilder = new DbContextOptionsBuilder<PatientPortalContext>();
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                
                using var context = new PatientPortalContext(optionsBuilder.Options);
                
                // Test connection by getting current counts
                int currentStaff = await context.Staff.CountAsync();
                int currentPatients = await context.Patients.CountAsync();
                
                Console.WriteLine($"✓ Connected to database");
                Console.WriteLine($"  Current Staff: {currentStaff}");
                Console.WriteLine($"  Current Patients: {currentPatients}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Database connection failed: {ex.Message}");
                return;
            }
        }
    }
}