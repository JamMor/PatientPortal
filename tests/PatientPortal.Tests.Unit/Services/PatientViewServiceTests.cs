using NSubstitute;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for empty result sets and null returns from IPatientService methods.
// TODO: Add validation tests for pagination boundaries (page out of range, zero/negative page sizes).
// TODO: Add tests for sorting fallback/default behavior in ReturnPatientManagerView.
public class PatientViewServiceTests
{
    private readonly IPatientService _mockPatientService;
    private readonly PatientViewService _patientViewService;

    public PatientViewServiceTests()
    {
        _mockPatientService = Substitute.For<IPatientService>();
        _patientViewService = new PatientViewService(_mockPatientService);
    }
    
        #region Helper Methods

        private static DateTime DaysAgo(int daysAgo)
        {
            var anchorDate = new DateTime(2024, 1, 1);
            return anchorDate.AddDays(-daysAgo);
        }

        private static Patient CreatePatient(
            int id,
            string firstName,
            string lastName,
            DateTime dob,
            string ssn,
            int createdDaysAgo = 0,
            string? phoneNumber = null,
            string? email = null,
            Address? address = null)
        {            
            var createdAt = DaysAgo(createdDaysAgo);
            return new Patient
            {
                PatientId = id,
                FirstName = firstName,
                LastName = lastName,
                DOB = dob,
                Last4SSN = ssn,
                CreatedAt = createdAt,
                Address = address,
                PhoneNumber = phoneNumber,
                Email = email,
                MessagingLink = new MessagingLink()
            };
        }
    
        // Helper for creating an Address
        private static Address CreateAddress(string street, string city, string state, string zip)
        {
            return new Address
            {
                StreetAddress = street,
                City = city,
                State = state,
                ZipCode = zip
            };
        }

        // Helper for creating a fully populated Patient
        private static Patient CreateFullPatient(
            int id,
            string firstName,
            string lastName,
            DateTime dob,
            string ssn,
            int createdDaysAgo = 0,
            int? updatedDaysAgo = null,
            string? phone = null,
            string? email = null,
            Address? address = null)
        {
            var patient = CreatePatient(id, firstName, lastName, dob, ssn, createdDaysAgo, phone, email, address);
            patient.UpdatedAt = updatedDaysAgo.HasValue ? DaysAgo(updatedDaysAgo.Value) : DaysAgo(createdDaysAgo-10);
            patient.MedicalTeam = new List<PatientStaffConnection>();
            patient.HealthIssues = new List<HealthIssue>();
            patient.Visits = new List<Visit>();
            patient.Tests = new List<PatientPortal.Models.TestResult>();
            return patient;
        }
    
    #endregion

    #region GetPatientInfoHeader Tests

    [Fact]
    public void GetPatientInfoHeader_WithValidPatientId_ReturnsPatientHeaderInfoView()
    {
        // Arrange
        var patientId = 1;
        var patients = new List<Patient>
        {
            CreatePatient(1, "John", "Doe", new DateTime(1990, 1, 1), "1234", 30),
            CreatePatient(2, "Jane", "Smith", new DateTime(1985, 5, 15), "5678", 60)
        }.AsQueryable();

        var firstPatient = patients.First();

        _mockPatientService.GetPatientBasicInfo().Returns(patients);

        // Act
        var result = _patientViewService.GetPatientInfoHeader(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CurrentPatientId);
        Assert.Equal(firstPatient.MessagingLink!.MessagingLinkId, result.CurrentPatientLinkId);
        Assert.Equal("John", result.CurrentPatientFirstName);
        Assert.Equal("Doe", result.CurrentPatientLastName);
        Assert.Equal("1234", result.CurrentPatientSSN);
        Assert.Equal(new DateTime(1990, 1, 1), result.CurrentPatientDOB);
        Assert.Equal(DaysAgo(30), result.CurrentPatientCreatedOn);
    }

    [Fact]
    public void GetPatientInfoHeader_WithNonExistingPatientId_ReturnsNull()
    {
        // Arrange
        var patientId = 999;
        var patients = new List<Patient>
        {
            CreatePatient(1, "John", "Doe", new DateTime(1990, 1, 1), "1234", 30),
        }.AsQueryable();

        _mockPatientService.GetPatientBasicInfo().Returns(patients);

        // Act
        var result = _patientViewService.GetPatientInfoHeader(patientId);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetPatientInfo Tests

    [Fact]
    public void GetPatientInfo_WithValidPatientId_ReturnsPatientInfoViewModel()
    {
        // Arrange
        var patientId = 1;
        var patients = new List<Patient>
        {
            CreateFullPatient(
                1,
                "Alice",
                "Johnson",
                new DateTime(1992, 3, 10),
                "9876",
                45,
                5,
                "555-9876",
                "alice.johnson@email.com",
                CreateAddress("123 Main St", "Anytown", "CA", "12345")
            )
        }.AsQueryable();

        _mockPatientService.GetPatientFullInfo().Returns(patients);

        // Act
        var result = _patientViewService.GetPatientInfo(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.PatientId);
        Assert.Equal(patients.First().MessagingLink!.MessagingLinkId, result.MessagingLinkId);
        Assert.Equal("Alice", result.FirstName);
        Assert.Equal("Johnson", result.LastName);
        Assert.Equal(new DateTime(1992, 3, 10), result.DOB);
        Assert.Equal("9876", result.Last4SSN);
        Assert.Equal("555-9876", result.PhoneNumber);
        Assert.Equal("alice.johnson@email.com", result.Email);

        // Check PatientHeader
        Assert.NotNull(result.PatientHeader);
        Assert.Equal(1, result.PatientHeader.CurrentPatientId);
        Assert.Equal("Alice", result.PatientHeader.CurrentPatientFirstName);
        Assert.Equal("Johnson", result.PatientHeader.CurrentPatientLastName);

        // Check Address
        Assert.NotNull(result.Address);
        Assert.Equal("123 Main St", result.Address.StreetAddress);
        Assert.Equal("Anytown", result.Address.City);
        Assert.Equal("CA", result.Address.State);
        Assert.Equal("12345", result.Address.ZipCode);
    }

    [Fact]
    public void GetPatientInfo_WithPatientWithoutAddress_ReturnsViewModelWithNullAddressFields()
    {
        // Arrange
        var patientId = 1;
        var patients = new List<Patient>
        {
            CreateFullPatient(
                1,
                "Bob",
                "Wilson",
                new DateTime(1988, 7, 20),
                "4321",
                20,
                2,
                "555-0000",
                "bob.wilson@email.com",
                null // No address
            )
        }.AsQueryable();

        _mockPatientService.GetPatientFullInfo().Returns(patients);

        // Act
        var result = _patientViewService.GetPatientInfo(patientId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Address);
    }

    #endregion

    #region ReturnPatientManagerView Tests

    [Fact]
    public void ReturnPatientManagerView_WithSearchAndPagination_ReturnsPatientManagerView()
    {
        // Arrange
        var searchQuery = new PatientSearch { SearchFirstName = "John" };
        var paginationSettings = new Paginator { CurrentPage = 1, ResultsPerPage = 10, SortOrder = "firstname_asc" };
        var staffId = 1;

        var searchResults = new List<Patient>
        {
            CreatePatient(1, "John", "Doe", new DateTime(1990, 1, 1), "1234"),
            CreatePatient(2, "John", "Smith", new DateTime(1985, 5, 15), "5678")
        }.AsQueryable();

        var sortedResults = searchResults.OrderBy(p => p.FirstName).AsQueryable();

        _mockPatientService.SearchPatients(searchQuery, staffId).Returns(searchResults);
        _mockPatientService.SortPatients(searchResults, paginationSettings.SortOrder).Returns(sortedResults);

        // Act
        var result = _patientViewService.ReturnPatientManagerView(searchQuery, paginationSettings, staffId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(searchQuery, result.SearchBar);
        Assert.Equal(paginationSettings, result.PaginationSettings);
        Assert.Equal(2, result.PaginationSettings.ResultsCount);
        Assert.Equal(2, result.ResultsCount);
        Assert.NotNull(result.SearchResults);
        Assert.Equal(2, result.SearchResults.Count());

        var firstResult = result.SearchResults.First();
        Assert.Equal(1, firstResult.PatientId);
        Assert.Equal("John", firstResult.FirstName);
        Assert.Equal("Doe", firstResult.LastName);
        Assert.Equal("1234", firstResult.Last4SSN);
    }

    #endregion

}
