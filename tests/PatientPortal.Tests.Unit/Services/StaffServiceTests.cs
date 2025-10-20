using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.DTOs;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for invalid or incomplete StaffFormView input (e.g., missing username/password/role).
// TODO: Add tests for boundary values (e.g., very long names/usernames, duplicate usernames, invalid IDs).
// TODO: Add tests for security constraints (e.g., IsAdmin restrictions, password complexity/validation failures).
// TODO: Add tests for related-entity handling when deleting staff (e.g., messaging link or dependent records).
// TODO: Add tests for empty-database behaviors across search and sort operations.

public class StaffServiceTests : IDisposable
{
    private readonly PatientPortalContext _context;
    private readonly StaffService _staffService;

    public StaffServiceTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<PatientPortalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientPortalContext(options);
        _staffService = new StaffService(_context);
    }

    private static Staff CreateStaff(string firstName, string lastName, string username, string role, bool isAdmin = false, string password = "hashedpassword")
    {
        return new Staff
        {
            FirstName = firstName,
            LastName = lastName,
            StaffUsername = username,
            Password = password,
            Role = role,
            IsAdmin = isAdmin,
            MessagingLink = new MessagingLink()
        };
    }

    private void AddStaff(params Staff[] staffMembers)
    {
        _context.Staff.AddRange(staffMembers);
        _context.SaveChanges();
    }

    #region CRUD Tests

    // NOTE: DoesStaffExist removed - Identity framework now handles username uniqueness validation

    [Fact]
    public void CreateStaff_WithValidData_ReturnsStaff()
    {
        // Arrange
        var staffDTO = new StaffDTO("Jane", "Nurse", "Nurse");
        var identityUser = new IdentityUser { UserName = "jnurse" };

        // Act
        var createdStaff = _staffService.CreateStaff(staffDTO, identityUser);

        // Assert
        Assert.NotNull(createdStaff);
        Assert.True(createdStaff.StaffId > 0);
        Assert.Equal("Jane", createdStaff.FirstName);
        Assert.Equal("Nurse", createdStaff.LastName);
        Assert.Equal("Nurse", createdStaff.Role);
        Assert.False(createdStaff.IsAdmin);
        Assert.NotNull(createdStaff.MessagingLink);
        Assert.Equal(identityUser, createdStaff.User);
    }

    [Fact]
    public void DeleteStaff_WithExistingStaff_RemovesStaff()
    {
        // Arrange
        var staff = CreateStaff("Bob", "Admin", "badmin", "Administrator", true);
        AddStaff(staff);
        var staffId = staff.StaffId;

        // Act
        _staffService.DeleteStaff(staffId);

        // Assert
        var deletedStaff = _context.Staff.Find(staffId);
        Assert.Null(deletedStaff);
    }

    [Fact]
    public void DeleteStaff_WithNonExistingStaff_DoesNotThrow()
    {
        // Arrange
        var nonExistingStaffId = 99999;

        // Act & Assert
        var exception = Record.Exception(() => _staffService.DeleteStaff(nonExistingStaffId));
        Assert.Null(exception);
    }

    [Fact]
    public void GetStaffbyId_WithExistingStaff_ReturnsStaff()
    {
        // Arrange
        var staff = CreateStaff("Alice", "Therapist", "atherapist", "Physical Therapist");
        AddStaff(staff);

        // Act
        var result = _staffService.GetStaffbyId(staff.StaffId);

        // Assert
        Assert.Single(result);
        var retrievedStaff = result.First();
        Assert.Equal("Alice", retrievedStaff.FirstName);
        Assert.Equal("Therapist", retrievedStaff.LastName);
        Assert.Equal("atherapist", retrievedStaff.StaffUsername);
    }

    [Fact]
    public void GetStaffbyId_WithNonExistingStaff_ReturnsEmpty()
    {
        // Act
        var result = _staffService.GetStaffbyId(99999);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region SearchStaff Tests

    [Fact]
    public void SearchStaff_WithFirstNameMatch_ReturnsMatchingStaff()
    {
        // Arrange
        var staff1 = CreateStaff("John", "Doe", "jdoe", "Doctor");
        var staff2 = CreateStaff("Jane", "Smith", "jsmith", "Nurse");
        AddStaff(staff1, staff2);

        var searchParams = new StaffSearch { SearchFirstName = "John" };

        // Act
        var result = _staffService.SearchStaff(searchParams);

        // Assert
        Assert.Single(result);
        Assert.Equal("John", result.First().FirstName);
    }

    [Fact]
    public void SearchStaff_ByStaffId_ReturnsMatchingStaff()
    {
        // Arrange
        var staff1 = CreateStaff("John", "Doe", "jdoe", "Doctor");
        var staff2 = CreateStaff("Jane", "Smith", "jsmith", "Nurse");
        AddStaff(staff1, staff2);

        var searchParams = new StaffSearch { SearchStaffId = staff1.StaffId };

        // Act
        var result = _staffService.SearchStaff(searchParams).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("John", result.First().FirstName);
    }

    [Fact]
    public void SearchStaff_ByRole_ReturnsAllStaffWithRole()
    {
        // Arrange
        var staff1 = CreateStaff("John", "Doe", "jdoe", "Doctor");
        var staff2 = CreateStaff("Jane", "Smith", "jsmith", "Doctor");
        var staff3 = CreateStaff("Bob", "Wilson", "bwilson", "Nurse");
        AddStaff(staff1, staff2, staff3);

        var searchParams = new StaffSearch { SearchRole = "Doctor" };

        // Act
        var result = _staffService.SearchStaff(searchParams).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, s => Assert.Equal("Doctor", s.Role));
    }

    [Fact]
    public void SearchStaff_WithMultipleCriteria_ReturnsIntersection()
    {
        // Arrange
        var staff1 = CreateStaff("John", "Smith", "jsmith1", "Doctor");
        var staff2 = CreateStaff("John", "Doe", "jdoe", "Nurse");
        var staff3 = CreateStaff("Jane", "Smith", "jsmith2", "Doctor");
        AddStaff(staff1, staff2, staff3);

        var searchParams = new StaffSearch { SearchFirstName = "John", SearchRole = "Doctor" };

        // Act
        var result = _staffService.SearchStaff(searchParams).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("Smith", result.First().LastName);
    }

    [Fact]
    public void SearchStaff_WithEmptySearch_ReturnsAllStaff()
    {
        // Arrange
        var staff1 = CreateStaff("John", "Doe", "jdoe", "Doctor");
        var staff2 = CreateStaff("Jane", "Smith", "jsmith", "Nurse");
        AddStaff(staff1, staff2);

        var searchParams = new StaffSearch();

        // Act
        var result = _staffService.SearchStaff(searchParams).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void SearchStaff_WithNoMatches_ReturnsEmptyList()
    {
        // Arrange
        var staff = CreateStaff("John", "Doe", "jdoe", "Doctor");
        AddStaff(staff);

        var searchParams = new StaffSearch { SearchFirstName = "NonExistent" };

        // Act
        var result = _staffService.SearchStaff(searchParams).ToList();

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region SortStaff Tests

    [Fact]
    public void SortStaff_WithLastNameAscending_ReturnsSortedStaff()
    {
        // Arrange
        var staff1 = CreateStaff("John", "Zebra", "jzebra", "Doctor", false);
        var staff2 = CreateStaff("Jane", "Apple", "japple", "Nurse", false);
        AddStaff(staff1, staff2);

        var query = _context.Staff.AsQueryable();

        // Act
        var result = _staffService.SortStaff(query, "lastname_asc").ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Apple", result[0].LastName);
        Assert.Equal("Zebra", result[1].LastName);
    }

    [Fact]
    public void SortStaff_ByStaffIdDescending_ReturnsSortedStaff()
    {
        // Arrange
        var staff1 = CreateStaff("First", "Staff", "first", "Doctor");
        var staff2 = CreateStaff("Second", "Staff", "second", "Doctor");
        AddStaff(staff1, staff2);

        var query = _context.Staff.AsQueryable();

        // Act
        var result = _staffService.SortStaff(query, "StaffId_desc").ToList();

        // Assert
        Assert.Equal(staff2.StaffId, result.First().StaffId);
    }

    [Fact]
    public void SortStaff_ByRoleAscending_ReturnsSortedStaff()
    {
        // Arrange
        var staff1 = CreateStaff("John", "Doe", "jdoe", "Nurse");
        var staff2 = CreateStaff("Jane", "Smith", "jsmith", "Doctor");
        AddStaff(staff1, staff2);

        var query = _context.Staff.AsQueryable();

        // Act
        var result = _staffService.SortStaff(query, "Role_asc").ToList();

        // Assert
        Assert.Equal("Doctor", result.First().Role);
        Assert.Equal("Nurse", result.Last().Role);
    }

    [Fact]
    public void SortStaff_WithDefaultSort_SortsByLastNameAscending()
    {
        // Arrange
        var staff1 = CreateStaff("John", "Zebra", "jzebra", "Doctor");
        var staff2 = CreateStaff("Jane", "Apple", "japple", "Nurse");
        AddStaff(staff1, staff2);

        var query = _context.Staff.AsQueryable();

        // Act
        var result = _staffService.SortStaff(query, "invalid_sort").ToList();

        // Assert
        Assert.Equal("Apple", result.First().LastName);
    }

    #endregion

    #region Edge Case Tests

    // NOTE: Password hashing is now handled by Identity framework in AuthService, not StaffService

    [Fact]
    public void CreateStaff_SetsIsAdminFalse_ByDefault()
    {
        // Arrange
        var staffDTO = new StaffDTO("New", "Staff", "Doctor");
        var identityUser = new IdentityUser { UserName = "newstaff" };

        // Act
        var createdStaff = _staffService.CreateStaff(staffDTO, identityUser);

        // Assert
        Assert.NotNull(createdStaff);
        Assert.False(createdStaff.IsAdmin);
    }

    [Fact]
    public void CreateStaff_CreatesMessagingLink_Automatically()
    {
        // Arrange
        var staffDTO = new StaffDTO("Link", "Test", "Nurse");
        var identityUser = new IdentityUser { UserName = "linktest" };

        // Act
        var createdStaff = _staffService.CreateStaff(staffDTO, identityUser);

        // Assert
        var savedStaff = _context.Staff
            .Include(s => s.MessagingLink)
            .FirstOrDefault(s => s.StaffId == createdStaff.StaffId);
        Assert.NotNull(savedStaff);
        Assert.NotNull(savedStaff.MessagingLink);
    }

    [Fact]
    public void GetStaffbyId_WithNoStaff_ReturnsEmptyQueryable()
    {
        // Act
        var result = _staffService.GetStaffbyId(99999);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}
