using Microsoft.EntityFrameworkCore;
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

    #region CRUD Tests
    
    [Fact]
    public void DoesPatientExist_WithExistingPatient_ReturnsTrue()
    {
        // Arrange
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            DOB = new DateTime(1990, 1, 1),
            Last4SSN = "1234",
            PhoneNumber = "555-1234",
            Email = "john.doe@email.com",
            MessagingLink = new MessagingLink()
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();

        var patientFormView = new PatientFormView
        {
            FirstName = "John",
            LastName = "Doe",
            DOB = new DateTime(1990, 1, 1),
            Last4SSN = "1234"
        };

        // Act
        var result = _patientService.DoesPatientExist(patientFormView);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void DoesPatientExist_WithNonExistingPatient_ReturnsFalse()
    {
        // Arrange
        var patientFormView = new PatientFormView
        {
            FirstName = "Jane",
            LastName = "Smith",
            DOB = new DateTime(1985, 5, 15),
            Last4SSN = "5678"
        };

        // Act
        var result = _patientService.DoesPatientExist(patientFormView);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreatePatient_WithValidData_ReturnsPatientId()
    {
        // Arrange
        var patientFormView = new PatientFormView
        {
            FirstName = "Alice",
            LastName = "Johnson",
            DOB = new DateTime(1992, 3, 10),
            Last4SSN = "9876",
            PhoneNumber = "555-9876",
            Email = "alice.johnson@email.com"
        };

        // Act
        var patientId = _patientService.CreatePatient(patientFormView);

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
        var patientFormView = new PatientFormView
        {
            FirstName = "Bob",
            LastName = "Wilson",
            DOB = new DateTime(1988, 7, 20),
            Last4SSN = "4321",
            PhoneNumber = "555-4321",
            Email = "bob.wilson@email.com",
            Address = new AddressFormView
            {
                StreetAddress = "123 Main St",
                City = "Anytown",
                State = "CA",
                ZipCode = "12345"
            }
        };

        // Act
        var patientId = _patientService.CreatePatient(patientFormView);

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
        var patient = new Patient
        {
            FirstName = "Charlie",
            LastName = "Brown",
            DOB = new DateTime(1995, 12, 25),
            Last4SSN = "1111",
            MessagingLink = new MessagingLink()
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();
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
        var patient = new Patient
        {
            FirstName = "Diana",
            LastName = "Prince",
            DOB = new DateTime(1985, 6, 1),
            Last4SSN = "2222",
            MessagingLink = new MessagingLink()
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();

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
        var patient = new Patient
        {
            FirstName = "Bruce",
            LastName = "Wayne",
            DOB = new DateTime(1980, 2, 19),
            Last4SSN = "3333",
            MessagingLink = new MessagingLink(),
            HealthIssues = new List<HealthIssue>(),
            Visits = new List<Visit>(),
            Tests = new List<PatientPortal.Models.TestResult>(),
            MedicalTeam = new List<PatientStaffConnection>()
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();

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
        var patient1 = new Patient { FirstName = "John", LastName = "Doe", DOB = new DateTime(1990, 1, 1), Last4SSN = "1234", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "Jane", LastName = "Smith", DOB = new DateTime(1985, 5, 15), Last4SSN = "5678", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2);
        _context.SaveChanges();

        var searchParams = new PatientSearch { SearchPatientId = patient1.PatientId };

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
        var patient1 = new Patient { FirstName = "John", LastName = "Doe", DOB = new DateTime(1990, 1, 1), Last4SSN = "1234", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "Johnny", LastName = "Smith", DOB = new DateTime(1985, 5, 15), Last4SSN = "5678", MessagingLink = new MessagingLink() };
        var patient3 = new Patient { FirstName = "Jane", LastName = "Wilson", DOB = new DateTime(1988, 3, 20), Last4SSN = "9012", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2, patient3);
        _context.SaveChanges();

        var searchParams = new PatientSearch { SearchFirstName = "John" };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Equal(2, result.Count); // "John" and "Johnny" both start with "John"
    }

    [Fact]
    public void SearchPatients_ByLastName_ReturnsMatchingPatients()
    {
        // Arrange
        var patient1 = new Patient { FirstName = "John", LastName = "Smith", DOB = new DateTime(1990, 1, 1), Last4SSN = "1234", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "Jane", LastName = "Smithson", DOB = new DateTime(1985, 5, 15), Last4SSN = "5678", MessagingLink = new MessagingLink() };
        var patient3 = new Patient { FirstName = "Bob", LastName = "Jones", DOB = new DateTime(1988, 3, 20), Last4SSN = "9012", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2, patient3);
        _context.SaveChanges();

        var searchParams = new PatientSearch { SearchLastName = "Smith" };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void SearchPatients_BySSN_ReturnsExactMatch()
    {
        // Arrange
        var patient1 = new Patient { FirstName = "John", LastName = "Doe", DOB = new DateTime(1990, 1, 1), Last4SSN = "1234", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "Jane", LastName = "Smith", DOB = new DateTime(1985, 5, 15), Last4SSN = "5678", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2);
        _context.SaveChanges();

        var searchParams = new PatientSearch { SearchSSN = "1234" };

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
        var patient1 = new Patient { FirstName = "John", LastName = "Doe", DOB = new DateTime(1990, 1, 1), Last4SSN = "1234", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "Jane", LastName = "Smith", DOB = new DateTime(1990, 1, 1), Last4SSN = "5678", MessagingLink = new MessagingLink() };
        var patient3 = new Patient { FirstName = "Bob", LastName = "Wilson", DOB = new DateTime(1985, 5, 15), Last4SSN = "9012", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2, patient3);
        _context.SaveChanges();

        var searchParams = new PatientSearch { SearchBirthdate = new DateTime(1990, 1, 1) };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void SearchPatients_WithMultipleCriteria_ReturnsIntersection()
    {
        // Arrange
        var patient1 = new Patient { FirstName = "John", LastName = "Smith", DOB = new DateTime(1990, 1, 1), Last4SSN = "1234", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "John", LastName = "Doe", DOB = new DateTime(1990, 1, 1), Last4SSN = "5678", MessagingLink = new MessagingLink() };
        var patient3 = new Patient { FirstName = "Jane", LastName = "Smith", DOB = new DateTime(1990, 1, 1), Last4SSN = "9012", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2, patient3);
        _context.SaveChanges();

        var searchParams = new PatientSearch { SearchFirstName = "John", SearchLastName = "Smith" };

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
        var patient = new Patient { FirstName = "John", LastName = "Doe", DOB = new DateTime(1990, 1, 1), Last4SSN = "1234", MessagingLink = new MessagingLink() };
        _context.Patients.Add(patient);
        _context.SaveChanges();

        var searchParams = new PatientSearch { SearchFirstName = "NonExistent" };

        // Act
        var result = _patientService.SearchPatients(searchParams, 1).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void SearchPatients_WithEmptySearch_ReturnsAllPatients()
    {
        // Arrange
        var patient1 = new Patient { FirstName = "John", LastName = "Doe", DOB = new DateTime(1990, 1, 1), Last4SSN = "1234", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "Jane", LastName = "Smith", DOB = new DateTime(1985, 5, 15), Last4SSN = "5678", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2);
        _context.SaveChanges();

        var searchParams = new PatientSearch(); // All fields null/empty

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
        var patient1 = new Patient { FirstName = "First", LastName = "Patient", DOB = new DateTime(1990, 1, 1), Last4SSN = "1111", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "Second", LastName = "Patient", DOB = new DateTime(1990, 1, 1), Last4SSN = "2222", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2);
        _context.SaveChanges();

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
        var patient1 = new Patient { FirstName = "John", LastName = "Zebra", DOB = new DateTime(1990, 1, 1), Last4SSN = "1111", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "Jane", LastName = "Apple", DOB = new DateTime(1990, 1, 1), Last4SSN = "2222", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2);
        _context.SaveChanges();

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
        var patient1 = new Patient { FirstName = "Young", LastName = "Patient", DOB = new DateTime(2000, 1, 1), Last4SSN = "1111", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "Old", LastName = "Patient", DOB = new DateTime(1950, 1, 1), Last4SSN = "2222", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2);
        _context.SaveChanges();

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
        var patient1 = new Patient { FirstName = "John", LastName = "Zebra", DOB = new DateTime(1990, 1, 1), Last4SSN = "1111", MessagingLink = new MessagingLink() };
        var patient2 = new Patient { FirstName = "Jane", LastName = "Apple", DOB = new DateTime(1990, 1, 1), Last4SSN = "2222", MessagingLink = new MessagingLink() };
        _context.Patients.AddRange(patient1, patient2);
        _context.SaveChanges();

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
        var patient = new Patient
        {
            FirstName = "John",
            LastName = "Doe",
            DOB = new DateTime(1990, 1, 1),
            Last4SSN = "1234",
            MessagingLink = new MessagingLink()
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();

        var patientFormView = new PatientFormView
        {
            FirstName = "John",
            LastName = "Doe",
            DOB = new DateTime(1990, 1, 1),
            Last4SSN = "9999" // Different SSN
        };

        // Act
        var result = _patientService.DoesPatientExist(patientFormView);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CreatePatient_WithNullAddress_CreatesPatientWithoutAddress()
    {
        // Arrange
        var patientFormView = new PatientFormView
        {
            FirstName = "NoAddress",
            LastName = "Patient",
            DOB = new DateTime(1990, 1, 1),
            Last4SSN = "0000",
            Address = null
        };

        // Act
        var patientId = _patientService.CreatePatient(patientFormView);

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
        _patientService?.Dispose();
        _context?.Dispose();
    }
}