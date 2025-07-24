using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.DataGenerators;

/// <summary>
/// Generates fake patient data with no database dependencies.
/// Creates patients with basic information and MessagingLink.
/// </summary>
public class PatientDataGenerator
{
    /// <summary>
    /// Generates a specified number of patients with basic information.
    /// </summary>
    /// <param name="count">Number of patients to generate</param>
    /// <returns>List of generated patients</returns>
    public List<Patient> GenerateNPatients(int count)
    {
        var faker = new Faker<Patient>()
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            // Generate DOB between 19 and 70 years ago to ensure patients are adults
            .RuleFor(p => p.DOB, f => f.Date.Between(DateTime.Today.AddYears(-70), DateTime.Today.AddYears(-19)))
            .RuleFor(p => p.Last4SSN, f => f.Random.String2(4, "0123456789"))
            .RuleFor(p => p.Email, (f, p) => f.Internet.Email(p.FirstName, p.LastName))
            // Records Created (joined practice) between age 18 and a month before today
            .RuleFor(p => p.CreatedAt, (f, p) => f.Date.Between(p.DOB.AddYears(18), DateTime.Today.AddMonths(-1)))
            // UpdatedAt between CreatedAt and today
            .RuleFor(p => p.UpdatedAt, (f, p) => f.Date.Between(p.CreatedAt, DateTime.Today))
            .RuleFor(p => p.MessagingLink, (f, p) => new MessagingLink
            {
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.CreatedAt
            });

        return faker.Generate(count);
    }
}
