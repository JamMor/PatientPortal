using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Features.StaffSeeding.DataGenerators;

/// <summary>
/// Generates fake staff data using Bogus library.
/// </summary>
public class StaffDataGenerator
{
    /// <summary>
    /// TODO: Remove when full migration to IdentityUser is complete.
    /// Default placeholders for generated staff members. Actual management is
    /// handled by IdentityUser and must meet Identity configuration requirements.
    private const string defaultPassword = "[Managed by Identity]";

    /// <summary>
    /// Generates a specified number of fake staff members with randomized details.
    /// </summary>
    /// <param name="count">Number of staff members to generate.</param>
    /// <returns>List of generated <see cref="Staff"/> objects.</returns>
    public List<Staff> GenerateNStaff(int count)
    {
        var staffFaker = CreateFaker(null, null, null);

        return staffFaker.Generate(count);
    }

    /// <summary>
    /// Generates a single <see cref="Staff"/> entity with a preset name and role.
    /// Dates, MessagingLink, and StaffUsername placeholder are randomized.
    /// </summary>
    /// <param name="firstName">The first name to assign to the staff member.</param>
    /// <param name="lastName">The last name to assign to the staff member.</param>
    /// <param name="role">The role to assign to the staff member (e.g., MD, RN, NP, LN).</param>
    /// <returns>A generated <see cref="Staff"/> object with the specified details.</returns>
    public Staff GenerateStaffWithDetails(string firstName, string lastName, string role)
    {
        var staffFaker = CreateFaker(firstName, lastName, role);
        return staffFaker.Generate();
    }

    /// <summary>
    /// Creates a <see cref="Faker{Staff}"/> instance for generating fake staff data.
    /// </summary>
    /// <param name="firstName">Optional first name to use; if null, a random name is generated.</param>
    /// <param name="lastName">Optional last name to use; if null, a random name is generated.</param>
    /// <param name="role">Optional role to use (e.g., MD, RN, NP, LN); if null, a random role is chosen.</param>
    /// <returns>A configured <see cref="Faker{Staff}"/> for generating staff entities.</returns>
    private static Faker<Staff> CreateFaker(string? firstName, string? lastName, string? role)
    {
        // Optionally specified rules
        var staffFaker = new Faker<Staff>()
            .RuleFor(s => s.FirstName, f => firstName ?? f.Name.FirstName())
            .RuleFor(s => s.LastName, f => lastName ?? f.Name.LastName())
            .RuleFor(s => s.Role, f => role ?? f.PickRandom("MD", "RN", "NP", "LN"));

        // Always random rules
        staffFaker
            .RuleFor(s => s.IsAdmin, false)
            .RuleFor(
                s => s.StaffUsername,
                (f, s) =>
                {
                    // TODO: Remove StaffUsername field after full migration to IdentityUser
                    // The real username is handled by Identity and generated in
                    // IdentityUserDataGenerator, this is just a placeholder to satisfy the model.
                    string username = f.Internet.UserName(s.FirstName, s.LastName) + "00$";
                    if (username.Length < 10)
                    {
                        username += f.Random.String2(10 - username.Length, "0123456789");
                    }
                    return username;
                }
            )
            // TODO: Remove Password field after full migration to IdentityUser.
            // This is just a placeholder to satisfy the model.
            .RuleFor(s => s.Password, defaultPassword)
            .RuleFor(s => s.MessagingLink, f => new MessagingLink())
            .RuleFor(
                s => s.CreatedAt,
                f => f.Date.Between(DateTime.Today.AddYears(-25), DateTime.Today.AddYears(-1))
            )
            .RuleFor(s => s.UpdatedAt, (f, s) => f.Date.Between(s.CreatedAt, DateTime.Today));

        return staffFaker;
    }
}
