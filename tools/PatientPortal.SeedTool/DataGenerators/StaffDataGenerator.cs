using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.DataGenerators;

/// <summary>
/// Generates fake staff data using Bogus library.
/// </summary>
public class StaffDataGenerator
{
    /// <summary>
    /// Default placeholders for generated staff members. Actual management is 
    /// handled by IdentityUser and must meet Identity configuration requirements.
    private const string defaultPassword = "[Managed by Identity]";

    /// <summary>
    /// Generates a list of fake staff members.
    /// </summary>
    /// <param name="count">Number of staff members to generate</param>
    /// <returns>List of generated Staff entities (not yet persisted)</returns>
    public List<Staff> GenerateNStaff(int count)
    {
        var staffFaker = new Faker<Staff>()
            .RuleFor(s => s.IsAdmin, false)
            .RuleFor(s => s.FirstName, f => f.Name.FirstName())
            .RuleFor(s => s.LastName, f => f.Name.LastName())
            .RuleFor(s => s.Role, f => f.PickRandom("MD", "RN", "NP", "LN"))
            .RuleFor(s => s.StaffUsername, (f, s) =>
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
            })
            // TODO: Remove Password field after full migration to IdentityUser. 
            // This is just a placeholder to satisfy the model.
            .RuleFor(s => s.Password, defaultPassword)
            .RuleFor(s => s.MessagingLink, f => new MessagingLink())
            .RuleFor(s => s.CreatedAt, f => f.Date.Between(DateTime.Today.AddYears(-25), DateTime.Today.AddYears(-1)))
            .RuleFor(s => s.UpdatedAt, (f, s) => f.Date.Between(s.CreatedAt, DateTime.Today));

        return staffFaker.Generate(count);
    }
}
