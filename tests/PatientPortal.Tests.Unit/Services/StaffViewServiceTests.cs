using NSubstitute;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for empty result sets and null returns from IStaffService methods.
// TODO: Add validation tests for pagination boundaries (page out of range, zero/negative page sizes).
// TODO: Add tests for sorting fallback/default behavior in ReturnStaffManagerView.
public class StaffViewServiceTests : IDisposable
{
    private readonly IStaffService _mockStaffService;
    private readonly StaffViewService _staffViewService;

    public StaffViewServiceTests()
    {
        _mockStaffService = Substitute.For<IStaffService>();
        _staffViewService = new StaffViewService(_mockStaffService);
    }

    #region Helper Methods

    private static Staff CreateStaff(
        int id,
        string firstName,
        string lastName,
        string username,
        string role,
        int patientCount = 0)
    {
        var staff = new Staff
        {
            StaffId = id,
            FirstName = firstName,
            LastName = lastName,
            StaffUsername = username,
            Password = "hashedpassword",
            Role = role,
            IsAdmin = false,
            MessagingLink = new MessagingLink()
        };

        for (int i = 0; i < patientCount; i++)
        {
            staff.Patients.Add(new PatientStaffConnection());
        }

        return staff;
    }

    #endregion

    #region ReturnStaffManagerView Tests

    [Fact]
    public void ReturnStaffManagerView_WithSearchAndPagination_ReturnsStaffManagerView()
    {
        // Arrange
        var searchQuery = new StaffSearch { SearchLastName = "Smith" };
        var paginationSettings = new Paginator { CurrentPage = 1, ResultsPerPage = 10, SortOrder = "LastName_asc" };

        var searchResults = new List<Staff>
        {
            CreateStaff(1, "Jane", "Smith", "jsmith", "Nurse"),
            CreateStaff(2, "John", "Smith", "johnsmith", "Doctor", 3)
        }.AsQueryable();

        var sortedResults = searchResults.OrderBy(s => s.LastName).AsQueryable();

        _mockStaffService.SearchStaff(searchQuery).Returns(searchResults);
        _mockStaffService.SortStaff(searchResults, paginationSettings.SortOrder).Returns(sortedResults);

        // Act
        var result = _staffViewService.ReturnStaffManagerView(searchQuery, paginationSettings);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(searchQuery, result.SearchBar);
        Assert.Equal(paginationSettings, result.PaginationSettings);
        Assert.Equal(2, result.PaginationSettings.ResultsCount);
        Assert.Equal(2, result.ResultsCount);
        Assert.NotNull(result.SearchResults);
        Assert.Equal(2, result.SearchResults.Count);
    }

    [Fact]
    public void ReturnStaffManagerView_ResultsCountMatchesPaginationSettingsResultsCount()
    {
        // Verifies the ResultsCount convenience property is consistent with PaginationSettings.ResultsCount.
        var searchQuery = new StaffSearch();
        var paginationSettings = new Paginator { CurrentPage = 1, ResultsPerPage = 10 };

        var searchResults = new List<Staff>
        {
            CreateStaff(1, "Alice", "Jones", "ajones", "Therapist"),
            CreateStaff(2, "Bob", "Brown", "bbrown", "Nurse"),
            CreateStaff(3, "Carol", "White", "cwhite", "Doctor")
        }.AsQueryable();

        _mockStaffService.SearchStaff(searchQuery).Returns(searchResults);
        _mockStaffService.SortStaff(searchResults, paginationSettings.SortOrder).Returns(searchResults);

        var result = _staffViewService.ReturnStaffManagerView(searchQuery, paginationSettings);

        Assert.Equal(result.PaginationSettings.ResultsCount, result.ResultsCount);
        Assert.Equal(3, result.ResultsCount);
    }

    [Fact]
    public void ReturnStaffManagerView_MapsStaffFieldsToStaffResult()
    {
        // Arrange
        var searchQuery = new StaffSearch();
        var paginationSettings = new Paginator { CurrentPage = 1, ResultsPerPage = 10 };

        var staff = CreateStaff(42, "Alice", "Jones", "ajones", "Physical Therapist", 5);
        var searchResults = new List<Staff> { staff }.AsQueryable();

        _mockStaffService.SearchStaff(searchQuery).Returns(searchResults);
        _mockStaffService.SortStaff(searchResults, paginationSettings.SortOrder).Returns(searchResults);

        // Act
        var result = _staffViewService.ReturnStaffManagerView(searchQuery, paginationSettings);

        // Assert
        var staffResult = result.SearchResults.Single();
        Assert.Equal(42, staffResult.StaffId);
        Assert.Equal("Alice Jones", staffResult.FullName);
        Assert.Equal("Physical Therapist", staffResult.Role);
        Assert.Equal(5, staffResult.PatientCount);
    }

    [Fact]
    public void ReturnStaffManagerView_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var searchQuery = new StaffSearch();
        var paginationSettings = new Paginator { CurrentPage = 2, ResultsPerPage = 2 };

        var searchResults = Enumerable.Range(1, 5)
            .Select(i => CreateStaff(i, "First", $"Last{i}", $"user{i}", "Nurse"))
            .AsQueryable();

        _mockStaffService.SearchStaff(searchQuery).Returns(searchResults);
        _mockStaffService.SortStaff(searchResults, paginationSettings.SortOrder).Returns(searchResults);

        // Act
        var result = _staffViewService.ReturnStaffManagerView(searchQuery, paginationSettings);

        // Assert
        Assert.Equal(5, result.ResultsCount);
        Assert.Equal(2, result.SearchResults.Count); // Page 2 of 2 per page from 5 = 2 items
    }

    #endregion

    public void Dispose()
    {
        _staffViewService?.Dispose();
    }
}
