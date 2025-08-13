using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Features.PatientSeeding.DataGenerators;

/// <summary>
/// Generates fake patient data using the Bogus library, including MessagingLink and other randomized details.
/// </summary>
public class PatientDataGenerator
{
    /// <summary>
    /// Generates a specified number of patients with randomized details.
    /// </summary>
    /// <param name="count">Number of patients to generate.</param>
    /// <returns>List of generated <see cref="Patient"/> objects.</returns>
    public List<Patient> GenerateNPatients(int count) =>
        CreateFaker(null, null, null, null, null).Generate(count);

    /// <summary>
    /// Generates a single <see cref="Patient"/> entity with preset name, date of birth, and email.
    /// Phone, SSN, MessagingLink, and record dates are randomized.
    /// </summary>
    /// <param name="firstName">The first name to assign to the patient.</param>
    /// <param name="lastName">The last name to assign to the patient.</param>
    /// <param name="dob">The date of birth to assign to the patient.</param>
    /// <param name="email">The email address to assign to the patient.</param>
    /// <returns>A generated <see cref="Patient"/> object with the specified details.</returns>
    public Patient GeneratePatientWithDetails(
        string firstName,
        string lastName,
        DateTime dob,
        string email
    ) => CreateFaker(firstName, lastName, dob, email, null).Generate();

    /// <summary>
    /// Creates a <see cref="Faker{Patient}"/> instance for generating fake patient data.
    /// </summary>
    /// <param name="firstName">Optional first name to use; if null, a random name is generated.</param>
    /// <param name="lastName">Optional last name to use; if null, a random name is generated.</param>
    /// <param name="dob">Optional date of birth to use; if null, a random adult DOB is generated.</param>
    /// <param name="email">Optional email to use; if null, a random email is generated.</param>
    /// <param name="ssn">Optional last 4 digits of SSN; if null, a random value is generated.</param>
    /// <returns>A configured <see cref="Faker{Patient}"/> for generating patient entities.</returns>
    private static Faker<Patient> CreateFaker(
        string? firstName,
        string? lastName,
        DateTime? dob,
        string? email,
        string? ssn
    )
    {
        // Optionally specified rules
        var patientFaker = new Faker<Patient>()
            .RuleFor(p => p.FirstName, f => firstName ?? f.Name.FirstName())
            .RuleFor(p => p.LastName, f => lastName ?? f.Name.LastName())
            .RuleFor(
                p => p.DOB,
                f =>
                    dob
                    ??
                    // Generate DOB between 19 and 70 years ago to ensure patients are adults
                    f.Date.Between(DateTime.Today.AddYears(-70), DateTime.Today.AddYears(-19))
            )
            .RuleFor(p => p.Last4SSN, f => ssn ?? f.Random.String2(4, "0123456789"))
            .RuleFor(p => p.Email, (f, p) => email ?? f.Internet.Email(p.FirstName, p.LastName));

        // Always random rules
        patientFaker
            .RuleFor(p => p.PhoneNumber, f => f.Phone.PhoneNumber("###-###-####"))
            // Records Created (joined practice) between age 18 and a month before today
            .RuleFor(
                p => p.CreatedAt,
                (f, p) => f.Date.Between(p.DOB.AddYears(18), DateTime.Today.AddMonths(-1))
            )
            // UpdatedAt between CreatedAt and today
            .RuleFor(p => p.UpdatedAt, (f, p) => f.Date.Between(p.CreatedAt, DateTime.Today))
            .RuleFor(
                p => p.MessagingLink,
                (f, p) => new MessagingLink { CreatedAt = p.CreatedAt, UpdatedAt = p.CreatedAt }
            );

        return patientFaker;
    }
}
