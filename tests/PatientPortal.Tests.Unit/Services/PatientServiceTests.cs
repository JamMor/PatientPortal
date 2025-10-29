using Microsoft.EntityFrameworkCore;
using PatientPortal.DTOs;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for invalid or negative input scenarios (e.g., missing or malformed data, invalid parameters).
// TODO: Add tests for boundary and edge cases (e.g., extremely large or small values, empty or null collections).
public class PatientServiceTests : IDisposable
{
    private readonly PatientPortalContext _context;
    private readonly PatientService _patientService;

    public PatientServiceTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<PatientPortalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientPortalContext(options);
        _patientService = new PatientService(_context);
    }

    private static Patient CreatePatient(
        string firstName,
        string lastName,
        DateTime dob,
        string last4Ssn,
        string? phoneNumber = null,
        string? email = null,
        Address? address = null
    )
    {
        return new Patient
        {
            FirstName = firstName,
            LastName = lastName,
            DOB = dob,
            Last4SSN = last4Ssn,
            PhoneNumber = phoneNumber,
            Email = email,
            Address = address,
            MessagingLink = new MessagingLink(),
        };
    }

    private void AddPatients(params Patient[] patients)
    {
        _context.Patients.AddRange(patients);
        _context.SaveChanges();
    }

    #region CRUD Tests

    [Fact]
    public void DoesPatientExist_WithExistingPatient_ReturnsTrue()
    {
        // Arrange
        var patient = CreatePatient("John", "Doe", new DateTime(1990, 1, 1), "1234", "555-1234", "john.doe@email.com");
        AddPatients(patient);

        var patientDTO = new PatientDTO("John", "Doe", new DateTime(1990, 1, 1), "1234", null, null, null);

        // Act
        var result = _patientService.DoesPatientExist(patientDTO);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesPatientExist_WithNonExistingPatient_ReturnsFalse()
    {
        // Arrange
        var patientDTO = new PatientDTO("Jane", "Smith", new DateTime(1985, 5, 15), "5678", null, null, null);

        // Act
        var result = _patientService.DoesPatientExist(patientDTO);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreatePatient_WithValidData_ReturnsPatientId()
    {
        // Arrange
        var patientDTO = new PatientDTO(
            "Alice",
            "Johnson",
            new DateTime(1992, 3, 10),
            "9876",
            "555-9876",
            "alice.johnson@email.com",
            null
        );

        // Act
        var patientId = _patientService.CreatePatient(patientDTO);

        // Assert
        Assert.True(patientId > 0);

        var createdPatient = _context.Patients.Find(patientId);
        Assert.NotNull(createdPatient);
        Assert.Equal("Alice", createdPatient.FirstName);
        Assert.Equal("Johnson", createdPatient.LastName);
        Assert.Equal(new DateTime(1992, 3, 10), createdPatient.DOB);
        Assert.Equal("9876", createdPatient.Last4SSN);
        Assert.NotNull(createdPatient.MessagingLink);
    }

    [Fact]
    public void CreatePatient_WithAddress_CreatesPatientWithAddress()
    {
        // Arrange
        var patientDTO = new PatientDTO(
            "Bob",
            "Wilson",
            new DateTime(1988, 7, 20),
            "4321",
            "555-4321",
            "bob.wilson@email.com",
            new AddressDTO("123 Main St", "Anytown", "CA", "12345")
        );

        // Act
        var patientId = _patientService.CreatePatient(patientDTO);

        // Assert
        var createdPatient = _context.Patients
            .Include(p => p.Address)
            .FirstOrDefault(p => p.PatientId == patientId);

        Assert.NotNull(createdPatient);
        Assert.NotNull(createdPatient.Address);
        Assert.Equal("123 Main St", createdPatient.Address.StreetAddress);
        Assert.Equal("Anytown", createdPatient.Address.City);
        Assert.Equal("CA", createdPatient.Address.State);
        Assert.Equal("12345", createdPatient.Address.ZipCode);
    }

    [Fact]
    public void DeletePatient_WithExistingPatient_RemovesPatient()
    {
        // Arrange
        var patient = CreatePatient("Charlie", "Brown", new DateTime(1995, 12, 25), "1111");
        AddPatients(patient);
        var patientId = patient.PatientId;

        // Act
        _patientService.DeletePatient(patientId);

        // Assert
        var deletedPatient = _context.Patients.Find(patientId);
        Assert.Null(deletedPatient);
    }

    [Fact]
    public void DeletePatient_WithNonExistingPatient_DoesNotThrow()
    {
        // Arrange
        var nonExistingPatientId = 99999;

        // Act & Assert
        var exception = Record.Exception(() => _patientService.DeletePatient(nonExistingPatientId));
        Assert.Null(exception);
    }

    [Fact]
    public void GetPatientBasicInfo_ReturnsQueryableWithMessagingLink()
    {
        // Arrange
        var patient = CreatePatient("Diana", "Prince", new DateTime(1985, 6, 1), "2222");
        AddPatients(patient);

        // Act
        var result = _patientService.GetPatientBasicInfo();

        // Assert
        Assert.Single(result);
        var retrievedPatient = result.First();
        Assert.Equal("Diana", retrievedPatient.FirstName);
        Assert.Equal("Prince", retrievedPatient.LastName);
        Assert.NotNull(retrievedPatient.MessagingLink);
    }

    [Fact]
    public void GetPatientFullInfo_ReturnsQueryableWithAllIncludes()
    {
        // Arrange
        var patient = CreatePatient("Bruce", "Wayne", new DateTime(1980, 2, 19), "3333");
        patient.HealthIssues = new List<HealthIssue>();
        patient.Visits = new List<Visit>();
        patient.Tests = new List<PatientPortal.Models.TestResult>();
        patient.MedicalTeam = new List<PatientStaffConnection>();
        AddPatients(patient);

        // Act
        var result = _patientService.GetPatientFullInfo();

        // Assert
        Assert.Single(result);
        var retrievedPatient = result.First();
        Assert.Equal("Bruce", retrievedPatient.FirstName);
        Assert.Equal("Wayne", retrievedPatient.LastName);
        Assert.NotNull(retrievedPatient.MessagingLink);
        Assert.NotNull(retrievedPatient.HealthIssues);
        Assert.NotNull(retrievedPatient.Visits);
        Assert.NotNull(retrievedPatient.Tests);
        Assert.NotNull(retrievedPatient.MedicalTeam);
    }

    #endregion

    #region SearchPatients Tests

    [Fact]
    public void SearchPatients_ByPatientId_ReturnsMatchingPatient()
    {
        // Arrange
        var patient1 = CreatePatient("John", "Doe", new DateTime(1990, 1, 1), "1234");
        var patient2 = CreatePatient("Jane", "Smith", new DateTime(1985, 5, 15), "5678");
        AddPatients(patient1, patient2);

        var searchParams = new PatientFilter { PatientId = patient1.PatientId };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("John", result.First().FirstName);
    }

    [Fact]
    public void SearchPatients_ByFirstName_ReturnsMatchingPatients()
    {
        // Arrange
        var patient1 = CreatePatient("John", "Doe", new DateTime(1990, 1, 1), "1234");
        var patient2 = CreatePatient("Johnny", "Smith", new DateTime(1985, 5, 15), "5678");
        var patient3 = CreatePatient("Jane", "Wilson", new DateTime(1988, 3, 20), "9012");
        AddPatients(patient1, patient2, patient3);

        var searchParams = new PatientFilter { FirstName = "John" };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Equal(2, result.Count); // "John" and "Johnny" both start with "John"
    }

    [Fact]
    public void SearchPatients_ByLastName_ReturnsMatchingPatients()
    {
        // Arrange
        var patient1 = CreatePatient("John", "Smith", new DateTime(1990, 1, 1), "1234");
        var patient2 = CreatePatient("Jane", "Smithson", new DateTime(1985, 5, 15), "5678");
        var patient3 = CreatePatient("Bob", "Jones", new DateTime(1988, 3, 20), "9012");
        AddPatients(patient1, patient2, patient3);

        var searchParams = new PatientFilter { LastName = "Smith" };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void SearchPatients_BySSN_ReturnsExactMatch()
    {
        // Arrange
        var patient1 = CreatePatient("John", "Doe", new DateTime(1990, 1, 1), "1234");
        var patient2 = CreatePatient("Jane", "Smith", new DateTime(1985, 5, 15), "5678");
        AddPatients(patient1, patient2);

        var searchParams = new PatientFilter { SSN = "1234" };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("John", result.First().FirstName);
    }

    [Fact]
    public void SearchPatients_ByBirthdate_ReturnsMatchingPatients()
    {
        // Arrange
        var patient1 = CreatePatient("John", "Doe", new DateTime(1990, 1, 1), "1234");
        var patient2 = CreatePatient("Jane", "Smith", new DateTime(1990, 1, 1), "5678");
        var patient3 = CreatePatient("Bob", "Wilson", new DateTime(1985, 5, 15), "9012");
        AddPatients(patient1, patient2, patient3);

        var searchParams = new PatientFilter { Birthdate = new DateTime(1990, 1, 1) };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void SearchPatients_WithMultipleCriteria_ReturnsIntersection()
    {
        // Arrange
        var patient1 = CreatePatient("John", "Smith", new DateTime(1990, 1, 1), "1234");
        var patient2 = CreatePatient("John", "Doe", new DateTime(1990, 1, 1), "5678");
        var patient3 = CreatePatient("Jane", "Smith", new DateTime(1990, 1, 1), "9012");
        AddPatients(patient1, patient2, patient3);

        var searchParams = new PatientFilter { FirstName = "John", LastName = "Smith" };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("1234", result.First().Last4SSN);
    }

    [Fact]
    public void SearchPatients_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        var patient = CreatePatient("John", "Doe", new DateTime(1990, 1, 1), "1234");
        AddPatients(patient);

        var searchParams = new PatientFilter { FirstName = "NonExistent" };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void SearchPatients_WithEmptySearch_ReturnsAllPatients()
    {
        // Arrange
        var patient1 = CreatePatient("John", "Doe", new DateTime(1990, 1, 1), "1234");
        var patient2 = CreatePatient("Jane", "Smith", new DateTime(1985, 5, 15), "5678");
        AddPatients(patient1, patient2);

        var searchParams = new PatientFilter(); // All fields null/empty

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region SortPatients Tests

    [Fact]
    public void SortPatients_ByPatientIdDescending_ReturnsSortedResults()
    {
        // Arrange
        var patient1 = CreatePatient("First", "Patient", new DateTime(1990, 1, 1), "1111");
        var patient2 = CreatePatient("Second", "Patient", new DateTime(1990, 1, 1), "2222");
        AddPatients(patient1, patient2);

        var query = _context.Patients.AsQueryable();

        // Act
        var result = _patientService.SortPatients(query, "PatientId_desc").ToList();

        // Assert
        Assert.Equal(patient2.PatientId, result.First().PatientId);
    }

    [Fact]
    public void SortPatients_ByLastNameAscending_ReturnsSortedResults()
    {
        // Arrange
        var patient1 = CreatePatient("John", "Zebra", new DateTime(1990, 1, 1), "1111");
        var patient2 = CreatePatient("Jane", "Apple", new DateTime(1990, 1, 1), "2222");
        AddPatients(patient1, patient2);

        var query = _context.Patients.AsQueryable();

        // Act
        var result = _patientService.SortPatients(query, "LastName_asc").ToList();

        // Assert
        Assert.Equal("Apple", result.First().LastName);
        Assert.Equal("Zebra", result.Last().LastName);
    }

    [Fact]
    public void SortPatients_ByDOBDescending_ReturnsSortedResults()
    {
        // Arrange
        var patient1 = CreatePatient("Young", "Patient", new DateTime(2000, 1, 1), "1111");
        var patient2 = CreatePatient("Old", "Patient", new DateTime(1950, 1, 1), "2222");
        AddPatients(patient1, patient2);

        var query = _context.Patients.AsQueryable();

        // Act
        var result = _patientService.SortPatients(query, "DOB_desc").ToList();

        // Assert
        Assert.Equal("Young", result.First().FirstName);
    }

    [Fact]
    public void SortPatients_WithDefaultSort_SortsByLastNameAscending()
    {
        // Arrange
        var patient1 = CreatePatient("John", "Zebra", new DateTime(1990, 1, 1), "1111");
        var patient2 = CreatePatient("Jane", "Apple", new DateTime(1990, 1, 1), "2222");
        AddPatients(patient1, patient2);

        var query = _context.Patients.AsQueryable();

        // Act
        var result = _patientService.SortPatients(query, "invalid_sort_order").ToList();

        // Assert - Default is LastName ascending
        Assert.Equal("Apple", result.First().LastName);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void DoesPatientExist_WithPartialMatch_ReturnsFalse()
    {
        // Arrange
        var patient = CreatePatient("John", "Doe", new DateTime(1990, 1, 1), "1234");
        AddPatients(patient);

        var patientDTO = new PatientDTO(
            "John",
            "Doe",
            new DateTime(1990, 1, 1),
            "9999",
            null,
            null,
            null
        );

        // Act
        var result = _patientService.DoesPatientExist(patientDTO);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreatePatient_WithNullAddress_CreatesPatientWithoutAddress()
    {
        // Arrange
        var patientDTO = new PatientDTO(
            "NoAddress",
            "Patient",
            new DateTime(1990, 1, 1),
            "0000",
            null,
            null,
            null
        );

        // Act
        var patientId = _patientService.CreatePatient(patientDTO);

        // Assert
        var createdPatient = _context.Patients
            .Include(p => p.Address)
            .FirstOrDefault(p => p.PatientId == patientId);
        Assert.NotNull(createdPatient);
        Assert.Null(createdPatient.Address);
    }

    [Fact]
    public void GetPatientBasicInfo_WithNoPatients_ReturnsEmptyQueryable()
    {
        // Act
        var result = _patientService.GetPatientBasicInfo();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetPatientFullInfo_WithNoPatients_ReturnsEmptyQueryable()
    {
        // Act
        var result = _patientService.GetPatientFullInfo();

        // Assert
        Assert.Empty(result);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}
