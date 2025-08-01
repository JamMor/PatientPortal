using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.Features.StaffSeeding.DataGenerators;
public class IdentityUserDataGenerator
{
    /// <summary>
    /// Default password for IdentityUsers that matches the password 
    /// requirements of Identity configuration: 
    /// At least 10 chars, 1 digit, 1 lowercase, 1 uppercase, 1 non-alphanumeric
    /// </summary>
    private const string defaultPassword = "Password0$";

    public class IdentityUserProperties
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Returns a username and password for an IdentityUser that is compliant with
    /// the Identity configuration used in the main application.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <returns>An object containing the generated username and password for the IdentityUser.</returns>
    public IdentityUserProperties GenerateIdentityUserProperties(string firstName, string lastName)
    {
        var faker = new Faker();
        /// A (hopefully) unique username from Person's first and last name, 
        /// made to be at least 10 chars and use Identity-allowed characters:
        /// `abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+<>$`
        string username = faker.Internet.UserName(firstName, lastName) + "00$";
        username += "_Gen$" + faker.Random.String2(4, "0123456789");
        if (username.Length < 10)
        {
            username += faker.Random.String2(10 - username.Length, "0123456789");
        }

        return new IdentityUserProperties
        {
            Username = username,
            Password = defaultPassword
        };
    }
}