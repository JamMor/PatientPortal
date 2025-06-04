// TODO: TEMPORARY MIGRATION UTILITY - Remove this entire file after staff data migration is complete
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;

namespace PatientPortal.Services
{
    public class StaffPasswordMigration
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PatientPortalContext _context;

        public StaffPasswordMigration(UserManager<IdentityUser> userManager, PatientPortalContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task MigrateAsync()
        {
            Console.WriteLine("Starting staff data migration...");

            // Only migrate staff without linked users (idempotent)
            var staffToMigrate = _context.Staff
                .Where(s => s.User == null)
                .Include(s => s.MessagingLink)
                .ToList();

            if (!staffToMigrate.Any())
            {
                Console.WriteLine("No staff records to migrate. All staff already have linked users.");
                return;
            }

            Console.WriteLine($"Found {staffToMigrate.Count} staff records to migrate.");

            int successCount = 0;
            int failCount = 0;

            foreach (var staff in staffToMigrate)
            {
                try
                {
                    // Create Identity user with existing password hash
                    var identityUser = new IdentityUser
                    {
                        UserName = staff.StaffUsername,
                        NormalizedUserName = staff.StaffUsername.ToUpper(),
                        PasswordHash = staff.Password, // Direct copy - same hash format
                        SecurityStamp = Guid.NewGuid().ToString()
                    };

                    // Create user without password parameter (we're setting PasswordHash directly)
                    var result = await _userManager.CreateAsync(identityUser);

                    if (result.Succeeded)
                    {
                        // Link staff to new user
                        staff.User = identityUser;
                        successCount++;
                        Console.WriteLine($"Migrated staff {staff.StaffId}: {staff.FullName()}");
                    }
                    else
                    {
                        failCount++;
                        Console.WriteLine($"Failed to create user for staff {staff.StaffId}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                catch (Exception ex)
                {
                    failCount++;
                    Console.WriteLine($"Exception migrating staff {staff.StaffId}: {ex.Message}");
                }
            }

            // Save all staff-user links
            await _context.SaveChangesAsync();

            Console.WriteLine($"\nMigration complete: {successCount} succeeded, {failCount} failed.");
        }
    }
}
