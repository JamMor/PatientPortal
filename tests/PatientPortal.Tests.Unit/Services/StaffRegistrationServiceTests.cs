using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using PatientPortal.Infrastructure;
using PatientPortal.Interfaces;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// Tests for StaffRegistrationService covering the orchestration of Identity user creation and Staff record creation
public class StaffRegistrationServiceTests : IDisposable
{
    private readonly PatientPortalContext _context;
    private readonly IAuthService _authService;
    private readonly IStaffService _staffService;
    private readonly StaffRegistrationService _registrationService;

    public StaffRegistrationServiceTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<PatientPortalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientPortalContext(options);
        
        // Use real StaffService with in-memory database
        _staffService = new StaffService(_context);
        
        // Mock AuthService to control Identity behavior
        _authService = Substitute.For<IAuthService>();

        _registrationService = new StaffRegistrationService(_authService, _staffService, _context);
    }

    #region RegisterStaffAsync Tests

    [Fact]
    public async Task RegisterStaffAsync_WithValidData_CreatesUserAndStaff()
    {
        // Arrange
        var staffFormView = new StaffFormView
        {
            FirstName = "John",
            LastName = "Doctor",
            StaffUsername = "jdoctor",
            Password = "Password123!",
            Role = "Doctor"
        };

        var identityUser = new IdentityUser { UserName = "jdoctor", Id = "user-123" };
        var successResult = new ExtendedIdentityResult<IdentityUser>(
            IdentityResult.Success,
            identityUser
        );

        _authService.CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password)
            .Returns(successResult);

        // Act
        var result = await _registrationService.RegisterStaffAsync(staffFormView);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doctor", result.Value.LastName);
        Assert.Equal("Doctor", result.Value.Role);
        Assert.False(result.Value.IsAdmin);
        Assert.NotNull(result.Value.MessagingLink);
        Assert.Equal(identityUser, result.Value.User);

        // Verify staff was persisted to database
        var savedStaff = await _context.Staff
            .Include(s => s.MessagingLink)
            .FirstOrDefaultAsync(s => s.FirstName == "John", cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(savedStaff);
        await _authService.Received(1).CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password);
    }

    [Fact]
    public async Task RegisterStaffAsync_WithDuplicateUsername_ReturnsFailureWithoutCreatingStaff()
    {
        // Arrange
        var staffFormView = new StaffFormView
        {
            FirstName = "Jane",
            LastName = "Nurse",
            StaffUsername = "existinguser",
            Password = "Password123!",
            Role = "Nurse"
        };

        var identityError = new IdentityError
        {
            Code = "DuplicateUserName",
            Description = "Username 'existinguser' is already taken."
        };
        var failureResult = new ExtendedIdentityResult<IdentityUser>(
            IdentityResult.Failed(identityError),
            null
        );

        _authService.CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password)
            .Returns(failureResult);

        // Act
        var result = await _registrationService.RegisterStaffAsync(staffFormView);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Value);
        Assert.Contains(result.IdentityResult.Errors, e => e.Code == "DuplicateUserName");

        // Verify no staff was created
        var staffCount = await _context.Staff.CountAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(0, staffCount);
        await _authService.Received(1).CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password);
    }

    [Fact]
    public async Task RegisterStaffAsync_WithInvalidPassword_ReturnsFailureWithoutCreatingStaff()
    {
        // Arrange
        var staffFormView = new StaffFormView
        {
            FirstName = "Bob",
            LastName = "Admin",
            StaffUsername = "badmin",
            Password = "weak",
            Role = "Admin"
        };

        var identityErrors = new[]
        {
            new IdentityError { Code = "PasswordTooShort", Description = "Password must be at least 10 characters." },
            new IdentityError { Code = "PasswordRequiresUpper", Description = "Password must contain uppercase." }
        };
        var failureResult = new ExtendedIdentityResult<IdentityUser>(
            IdentityResult.Failed(identityErrors),
            null
        );

        _authService.CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password)
            .Returns(failureResult);

        // Act
        var result = await _registrationService.RegisterStaffAsync(staffFormView);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Value);
        Assert.Equal(2, result.IdentityResult.Errors.Count());
        Assert.Contains(result.IdentityResult.Errors, e => e.Code == "PasswordTooShort");

        // Verify no staff was created
        var staffCount = await _context.Staff.CountAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(0, staffCount);
    }

    [Fact]
    public async Task RegisterStaffAsync_SetsIsAdminToFalse()
    {
        // Arrange
        var staffFormView = new StaffFormView
        {
            FirstName = "Alice",
            LastName = "Therapist",
            StaffUsername = "atherapist",
            Password = "Password123!",
            Role = "Physical Therapist"
        };

        var identityUser = new IdentityUser { UserName = "atherapist" };
        var successResult = new ExtendedIdentityResult<IdentityUser>(
            IdentityResult.Success,
            identityUser
        );

        _authService.CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password)
            .Returns(successResult);

        // Act
        var result = await _registrationService.RegisterStaffAsync(staffFormView);

        // Assert
        Assert.True(result.Succeeded);
        Assert.False(result.Value!.IsAdmin);
    }

    [Fact]
    public async Task RegisterStaffAsync_CreatesMessagingLinkAutomatically()
    {
        // Arrange
        var staffFormView = new StaffFormView
        {
            FirstName = "Charlie",
            LastName = "Technician",
            StaffUsername = "ctech",
            Password = "Password123!",
            Role = "Lab Technician"
        };

        var identityUser = new IdentityUser { UserName = "ctech" };
        var successResult = new ExtendedIdentityResult<IdentityUser>(
            IdentityResult.Success,
            identityUser
        );

        _authService.CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password)
            .Returns(successResult);

        // Act
        var result = await _registrationService.RegisterStaffAsync(staffFormView);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value!.MessagingLink);
        
        // Verify MessagingLink is persisted
        var savedStaff = await _context.Staff
            .Include(s => s.MessagingLink)
            .FirstAsync(s => s.StaffUsername == "ctech", cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(savedStaff.MessagingLink);
        Assert.True(savedStaff.MessagingLink.MessagingLinkId > 0);
    }

    [Fact]
    public async Task RegisterStaffAsync_LinksStaffToIdentityUser()
    {
        // Arrange
        var staffFormView = new StaffFormView
        {
            FirstName = "Diana",
            LastName = "Counselor",
            StaffUsername = "dcounselor",
            Password = "Password123!",
            Role = "Counselor"
        };

        var identityUser = new IdentityUser { UserName = "dcounselor", Id = "user-456" };
        var successResult = new ExtendedIdentityResult<IdentityUser>(
            IdentityResult.Success,
            identityUser
        );

        _authService.CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password)
            .Returns(successResult);

        // Act
        var result = await _registrationService.RegisterStaffAsync(staffFormView);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value!.User);
        Assert.Equal("dcounselor", result.Value.User.UserName);
        Assert.Equal("user-456", result.Value.User.Id);
    }

    [Fact]
    public async Task RegisterStaffAsync_PopulatesStaffFromFormView()
    {
        // Arrange
        var staffFormView = new StaffFormView
        {
            FirstName = "Edward",
            LastName = "Specialist",
            StaffUsername = "especialist",
            Password = "Password123!",
            Role = "Specialist"
        };

        var identityUser = new IdentityUser { UserName = "especialist" };
        var successResult = new ExtendedIdentityResult<IdentityUser>(
            IdentityResult.Success,
            identityUser
        );

        _authService.CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password)
            .Returns(successResult);

        // Act
        var result = await _registrationService.RegisterStaffAsync(staffFormView);

        // Assert
        Assert.True(result.Succeeded);
        var staff = result.Value!;
        Assert.Equal(staffFormView.FirstName, staff.FirstName);
        Assert.Equal(staffFormView.LastName, staff.LastName);
        Assert.Equal(staffFormView.Role, staff.Role);
    }

    [Fact]
    public async Task RegisterStaffAsync_ReturnsStaffWithGeneratedId()
    {
        // Arrange
        var staffFormView = new StaffFormView
        {
            FirstName = "Frank",
            LastName = "Manager",
            StaffUsername = "fmanager",
            Password = "Password123!",
            Role = "Office Manager"
        };

        var identityUser = new IdentityUser { UserName = "fmanager" };
        var successResult = new ExtendedIdentityResult<IdentityUser>(
            IdentityResult.Success,
            identityUser
        );

        _authService.CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password)
            .Returns(successResult);

        // Act
        var result = await _registrationService.RegisterStaffAsync(staffFormView);

        // Assert
        Assert.True(result.Succeeded);
        Assert.True(result.Value!.StaffId > 0);
    }

    [Fact]
    public async Task RegisterStaffAsync_WithMultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var staffFormView = new StaffFormView
        {
            FirstName = "Grace",
            LastName = "Physician",
            StaffUsername = "invalid user!",
            Password = "bad",
            Role = "Physician"
        };

        var identityErrors = new[]
        {
            new IdentityError { Code = "InvalidUserName", Description = "Username contains invalid characters." },
            new IdentityError { Code = "PasswordTooShort", Description = "Password too short." },
            new IdentityError { Code = "PasswordRequiresDigit", Description = "Password requires digit." }
        };
        var failureResult = new ExtendedIdentityResult<IdentityUser>(
            IdentityResult.Failed(identityErrors),
            null
        );

        _authService.CreateUserAsync(staffFormView.StaffUsername, staffFormView.Password)
            .Returns(failureResult);

        // Act
        var result = await _registrationService.RegisterStaffAsync(staffFormView);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal(3, result.IdentityResult.Errors.Count());
        Assert.Contains(result.IdentityResult.Errors, e => e.Code == "InvalidUserName");
        Assert.Contains(result.IdentityResult.Errors, e => e.Code == "PasswordTooShort");
        Assert.Contains(result.IdentityResult.Errors, e => e.Code == "PasswordRequiresDigit");
    }

    #endregion

    public void Dispose()
    {
        _staffService?.Dispose();
        _context?.Dispose();
    }
}
