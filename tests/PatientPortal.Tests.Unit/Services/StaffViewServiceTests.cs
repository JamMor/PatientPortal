using NSubstitute;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for empty result sets and null returns from IStaffService methods.
// TODO: Add validation tests for pagination boundaries (page out of range, zero/negative page sizes).
// TODO: Add tests for sorting fallback/default behavior in ReturnStaffManagerView.
public class StaffViewServiceTests
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
        int patientCount = 0
    )
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
            MessagingLink = new MessagingLink(),
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
        var filter = new StaffFilter { LastName = "Smith" };
        string sortOrder = "LastName_asc";
        var paging = new Paginator();

        var searchResults = new List<Staff>
        {
            CreateStaff(1, "Jane", "Smith", "jsmith", "Nurse"),
            CreateStaff(2, "John", "Smith", "johnsmith", "Doctor", 3),
        }.AsQueryable();

        var sortedResults = searchResults.OrderBy(s => s.LastName).AsQueryable();

        _mockStaffService.SearchStaff(filter).Returns(searchResults);
        _mockStaffService.SortStaff(searchResults, sortOrder).Returns(sortedResults);

        // Act
        var result = _staffViewService.ReturnStaffManagerView(filter, paging, sortOrder);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Smith", result.Query.Filter.LastName);
        Assert.Equal(2, result.Query.Paging.ResultsCount);
        Assert.NotNull(result.Results.Staff);
        Assert.Equal(2, result.Results.Staff.Count);
    }

    [Fact]
    public void ReturnStaffManagerView_ResultsCountMatchesPaginationSettingsResultsCount()
    {
        // Verifies ResultsCount on Paging is set from query results.
        var filter = new StaffFilter();
        string sortOrder = "LastName_asc";
        var paging = new Paginator();

        var searchResults = new List<Staff>
        {
            CreateStaff(1, "Alice", "Jones", "ajones", "Therapist"),
            CreateStaff(2, "Bob", "Brown", "bbrown", "Nurse"),
            CreateStaff(3, "Carol", "White", "cwhite", "Doctor"),
        }.AsQueryable();

        _mockStaffService.SearchStaff(filter).Returns(searchResults);
        _mockStaffService.SortStaff(searchResults, sortOrder).Returns(searchResults);

        var result = _staffViewService.ReturnStaffManagerView(filter, paging, sortOrder);

        Assert.Equal(3, result.Query.Paging.ResultsCount);
    }

    [Fact]
    public void ReturnStaffManagerView_MapsStaffFieldsToStaffResult()
    {
        // Arrange
        var filter = new StaffFilter();
        string sortOrder = "LastName_asc";
        var paging = new Paginator();

        var staff = CreateStaff(42, "Alice", "Jones", "ajones", "Physical Therapist", 5);
        var searchResults = new List<Staff> { staff }.AsQueryable();

        _mockStaffService.SearchStaff(filter).Returns(searchResults);
        _mockStaffService.SortStaff(searchResults, sortOrder).Returns(searchResults);

        // Act
        var result = _staffViewService.ReturnStaffManagerView(filter, paging, sortOrder);

        // Assert
        var staffResult = result.Results.Staff.Single();
        Assert.Equal(42, staffResult.StaffId);
        Assert.Equal("Alice", staffResult.FirstName);
        Assert.Equal("Jones", staffResult.LastName);
        Assert.Equal("Physical Therapist", staffResult.Role);
        Assert.Equal(5, staffResult.PatientCount);
    }

    [Fact]
    public void ReturnStaffManagerView_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var filter = new StaffFilter();
        string sortOrder = "LastName_asc";
        var paging = new Paginator { ResultsPerPage = 2, CurrentPage = 2 };

        var searchResults = Enumerable
            .Range(1, 5)
            .Select(i => CreateStaff(i, "First", $"Last{i}", $"user{i}", "Nurse"))
            .AsQueryable();

        _mockStaffService.SearchStaff(filter).Returns(searchResults);
        _mockStaffService.SortStaff(searchResults, sortOrder).Returns(searchResults);

        // Act
        var result = _staffViewService.ReturnStaffManagerView(filter, paging, sortOrder);

        // Assert
        Assert.Equal(5, result.Query.Paging.ResultsCount);
        Assert.Equal(2, result.Results.Staff.Count); // Page 2 of 2 per page from 5 = 2 items
    }

    #endregion
}
