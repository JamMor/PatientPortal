using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for null/empty login payloads and whitespace usernames.
// TODO: Add tests for lockout/failed-attempt behavior if introduced.
// TODO: Add boundary tests for password hashing/length constraints and case-insensitive username option if desired.
public class LoginServiceTests : IDisposable
{
    private readonly PatientPortalContext _context;
    private readonly LoginService _loginService;

    private const string ValidUsername = "testuser";
    private const string ValidPassword = "password123";
    private const string AdminUsername = "admin";
    private const string AdminPassword = "adminpass";
    private const string NurseUsername = "jnurse";
    private const string NursePassword = "password123";
    private const string WrongPassword = "wrongpassword";
    private const string CorrectPassword = "correctpassword";
    private const string NonexistentUsername = "nonexistentuser";

    public LoginServiceTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<PatientPortalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientPortalContext(options);
        _loginService = new LoginService(_context);
    }

    #region Helper Methods

    private Staff CreateStaff(
        string username,
        string password,
        string firstName = "John",
        string lastName = "Doe",
        string role = "Doctor",
        bool isAdmin = false)
    {
        var hasher = new PasswordHasher<Staff>();
        var tempStaff = new Staff();
        var hashedPassword = hasher.HashPassword(tempStaff, password);

        return new Staff
        {
            FirstName = firstName,
            LastName = lastName,
            StaffUsername = username,
            Password = hashedPassword,
            Role = role,
            IsAdmin = isAdmin,
            MessagingLink = new MessagingLink()
        };
    }

    private LoginStaff CreateLoginStaff(string username, string password)
    {
        return new LoginStaff
        {
            StaffUsername = username,
            LoginPassword = password
        };
    }

    #endregion

    #region DoesStaffUserExist Tests

    [Fact]
    public void DoesStaffUserExist_ExistingUsername_ReturnsStaff()
    {
        // Arrange
        var staff = CreateStaff(ValidUsername, ValidPassword);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        // Act
        var result = _loginService.DoesStaffUserExist(ValidUsername);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ValidUsername, result.StaffUsername);
        Assert.Equal(staff.FirstName, result.FirstName);
        Assert.NotNull(result.MessagingLink);
    }

    [Fact]
    public void DoesStaffUserExist_NonexistentUsername_ReturnsNull()
    {
        // Act
        var result = _loginService.DoesStaffUserExist(NonexistentUsername);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region VerifyStaffPassword Tests

    [Fact]
    public void VerifyStaffPassword_CorrectPassword_ReturnsTrue()
    {
        // Arrange
        var loginStaff = CreateLoginStaff(ValidUsername, ValidPassword);
        var savedStaff = CreateStaff(ValidUsername, ValidPassword);

        // Act
        var result = _loginService.VerifyStaffPassword(loginStaff, savedStaff);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyStaffPassword_IncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var loginStaff = CreateLoginStaff(ValidUsername, WrongPassword);
        var savedStaff = CreateStaff(ValidUsername, CorrectPassword);

        // Act
        var result = _loginService.VerifyStaffPassword(loginStaff, savedStaff);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region AttemptStaffLogin Tests

    [Fact]
    public void AttemptStaffLogin_ValidCredentials_ReturnsLoginStaffDTO()
    {
        // Arrange
        var staff = CreateStaff(NurseUsername, NursePassword, "Jane", "Nurse", "Nurse", false);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        var loginStaff = CreateLoginStaff(NurseUsername, NursePassword);

        // Act
        var result = _loginService.AttemptStaffLogin(loginStaff);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(staff.StaffId, result.StaffId);
        Assert.Equal(staff.MessagingLink.MessagingLinkId, result.MessagingLinkId);
        Assert.Equal(staff.IsAdmin, result.IsAdmin);
        Assert.Equal(staff.FullName(), result.FullName);
        Assert.Equal(staff.Role, result.Role);
    }

    [Fact]
    public void AttemptStaffLogin_InvalidUsername_ReturnsNull()
    {
        // Arrange
        var loginStaff = CreateLoginStaff(NonexistentUsername, ValidPassword);

        // Act
        var result = _loginService.AttemptStaffLogin(loginStaff);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void AttemptStaffLogin_InvalidPassword_ReturnsNull()
    {
        // Arrange
        var staff = CreateStaff("newadmin", CorrectPassword, "Bob", "Admin", "Administrator", true);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        var loginStaff = CreateLoginStaff("newadmin", WrongPassword);

        // Act
        var result = _loginService.AttemptStaffLogin(loginStaff);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void AttemptStaffLogin_AdminUser_ReturnsAdminDTO()
    {
        // Arrange
        var staff = CreateStaff(AdminUsername, AdminPassword, "Admin", "User", "Administrator", true);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        var loginStaff = CreateLoginStaff(AdminUsername, AdminPassword);

        // Act
        var result = _loginService.AttemptStaffLogin(loginStaff);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsAdmin);
        Assert.Equal("Administrator", result.Role);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void DoesStaffUserExist_WithEmptyUsername_ReturnsNull()
    {
        // Arrange
        var staff = CreateStaff(ValidUsername, ValidPassword);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        // Act
        var result = _loginService.DoesStaffUserExist("");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void DoesStaffUserExist_IncludesMessagingLink()
    {
        // Arrange
        var staff = CreateStaff(ValidUsername, ValidPassword);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        // Act
        var result = _loginService.DoesStaffUserExist(ValidUsername);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.MessagingLink);
    }

    [Fact]
    public void VerifyStaffPassword_WithEmptyPassword_ReturnsFalse()
    {
        // Arrange
        var loginStaff = CreateLoginStaff(ValidUsername, "");
        var savedStaff = CreateStaff(ValidUsername, ValidPassword);

        // Act
        var result = _loginService.VerifyStaffPassword(loginStaff, savedStaff);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AttemptStaffLogin_ReturnsCorrectMessagingLinkId()
    {
        // Arrange
        var staff = CreateStaff(ValidUsername, ValidPassword);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        var loginStaff = CreateLoginStaff(ValidUsername, ValidPassword);

        // Act
        var result = _loginService.AttemptStaffLogin(loginStaff);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(staff.MessagingLink.MessagingLinkId, result.MessagingLinkId);
    }

    [Fact]
    public void AttemptStaffLogin_ReturnsCorrectFullName()
    {
        // Arrange
        var staff = CreateStaff(ValidUsername, ValidPassword, "John", "Doe", "Doctor", false);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        var loginStaff = CreateLoginStaff(ValidUsername, ValidPassword);

        // Act
        var result = _loginService.AttemptStaffLogin(loginStaff);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public void AttemptStaffLogin_NonAdminUser_IsAdminFalse()
    {
        // Arrange
        var staff = CreateStaff(ValidUsername, ValidPassword, "Regular", "User", "Nurse", false);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        var loginStaff = CreateLoginStaff(ValidUsername, ValidPassword);

        // Act
        var result = _loginService.AttemptStaffLogin(loginStaff);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsAdmin);
    }

    [Fact]
    public void VerifyStaffPassword_WithWhitespacePassword_ReturnsFalse()
    {
        // Arrange
        var loginStaff = CreateLoginStaff(ValidUsername, "   ");
        var savedStaff = CreateStaff(ValidUsername, ValidPassword);

        // Act
        var result = _loginService.VerifyStaffPassword(loginStaff, savedStaff);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AttemptStaffLogin_WithCaseSensitiveUsername_ReturnsNullForWrongCase()
    {
        // Arrange
        var staff = CreateStaff("testuser", ValidPassword);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        var loginStaff = CreateLoginStaff("TESTUSER", ValidPassword);

        // Act
        var result = _loginService.AttemptStaffLogin(loginStaff);

        // Assert - Username comparison is case-sensitive in EF Core in-memory
        Assert.Null(result);
    }

    [Fact]
    public void AttemptStaffLogin_ReturnsAllDTOFields()
    {
        // Arrange
        var staff = CreateStaff(ValidUsername, ValidPassword, "Test", "User", "Physical Therapist", false);
        _context.Staff.Add(staff);
        _context.SaveChanges();

        var loginStaff = CreateLoginStaff(ValidUsername, ValidPassword);

        // Act
        var result = _loginService.AttemptStaffLogin(loginStaff);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(staff.StaffId, result.StaffId);
        Assert.Equal(staff.MessagingLink.MessagingLinkId, result.MessagingLinkId);
        Assert.Equal(staff.IsAdmin, result.IsAdmin);
        Assert.Equal("Test User", result.FullName);
        Assert.Equal("Physical Therapist", result.Role);
    }

    #endregion

    public void Dispose()
    {
        _loginService?.Dispose();
        _context?.Dispose();
    }
}
