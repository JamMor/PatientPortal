using Bogus;
using PatientPortal.Models;

namespace PatientPortal.SeedTool.DataGenerators;

/// <summary>
/// Generates fake address data.
/// </summary>
public class AddressDataGenerator
{
    /// <summary>
    /// Generates a single address for a patient.
    /// </summary>
    /// <returns>Single address object</returns>
    public Address GenerateAddress()
    {
        var faker = new Faker<Address>()
            .RuleFor(a => a.StreetAddress, f => f.Address.StreetAddress())
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.State, f => f.Address.State())
            .RuleFor(a => a.ZipCode, f => f.Address.ZipCode());

        return faker.Generate();
    }
}
