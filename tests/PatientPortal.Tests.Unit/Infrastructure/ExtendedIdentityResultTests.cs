using Microsoft.AspNetCore.Identity;
using PatientPortal.Infrastructure;

namespace PatientPortal.Tests.Unit.Infrastructure;

// Tests for ExtendedIdentityResult wrapper and error mapping functionality
public class ExtendedIdentityResultTests
{
    private readonly IdentityErrorDescriber _errorDescriber = new();

    #region MapIdentityErrorsToFields Tests

    [Fact]
    public void MapIdentityErrorsToFields_WithUsernameErrors_MapsToUsernameField()
    {
        // Arrange
        var errors = new[]
        {
            _errorDescriber.DuplicateUserName("testuser"),
            _errorDescriber.InvalidUserName("test@user")
        };
        var identityResult = IdentityResult.Failed(errors);
        var extendedResult = new ExtendedIdentityResult<IdentityUser>(identityResult, null);

        // Act
        var errorDict = extendedResult.MapIdentityErrorsToFields(
            usernameField: "StaffUsername",
            passwordField: "Password",
            confirmPasswordField: "ConfirmPassword"
        );

        // Assert
        Assert.Single(errorDict);
        Assert.True(errorDict.ContainsKey("StaffUsername"));
        Assert.Equal(2, errorDict["StaffUsername"].Count);
        Assert.Contains(errors[0].Description, errorDict["StaffUsername"]);
        Assert.Contains(errors[1].Description, errorDict["StaffUsername"]);
    }

    [Fact]
    public void MapIdentityErrorsToFields_WithPasswordErrors_MapsToPasswordField()
    {
        // Arrange
        var errors = new[]
        {
            _errorDescriber.PasswordTooShort(10),
            _errorDescriber.PasswordRequiresUpper(),
            _errorDescriber.PasswordRequiresDigit()
        };
        var identityResult = IdentityResult.Failed(errors);
        var extendedResult = new ExtendedIdentityResult<IdentityUser>(identityResult, null);

        // Act
        var errorDict = extendedResult.MapIdentityErrorsToFields(
            usernameField: "StaffUsername",
            passwordField: "Password",
            confirmPasswordField: "ConfirmPassword"
        );

        // Assert
        Assert.Single(errorDict);
        Assert.True(errorDict.ContainsKey("Password"));
        Assert.Equal(3, errorDict["Password"].Count);
        Assert.Contains(errors[0].Description, errorDict["Password"]);
        Assert.Contains(errors[1].Description, errorDict["Password"]);
        Assert.Contains(errors[2].Description, errorDict["Password"]);
    }

    [Fact]
    public void MapIdentityErrorsToFields_WithMismatchError_MapsToConfirmPasswordField()
    {
        // Arrange
        var errors = new[]
        {
            _errorDescriber.PasswordMismatch()
        };
        var identityResult = IdentityResult.Failed(errors);
        var extendedResult = new ExtendedIdentityResult<IdentityUser>(identityResult, null);

        // Act
        var errorDict = extendedResult.MapIdentityErrorsToFields(
            usernameField: "StaffUsername",
            passwordField: "Password",
            confirmPasswordField: "ConfirmPassword"
        );

        // Assert
        Assert.Single(errorDict);
        Assert.True(errorDict.ContainsKey("ConfirmPassword"));
        Assert.Single(errorDict["ConfirmPassword"]);
        Assert.Contains(errors[0].Description, errorDict["ConfirmPassword"]);
    }

    [Fact]
    public void MapIdentityErrorsToFields_WithGenericErrors_MapsToEmptyString()
    {
        // Arrange
        var errors = new[]
        {
            _errorDescriber.ConcurrencyFailure(),
            _errorDescriber.DefaultError()
        };
        var identityResult = IdentityResult.Failed(errors);
        var extendedResult = new ExtendedIdentityResult<IdentityUser>(identityResult, null);

        // Act
        var errorDict = extendedResult.MapIdentityErrorsToFields(
            usernameField: "StaffUsername",
            passwordField: "Password",
            confirmPasswordField: "ConfirmPassword"
        );

        // Assert
        Assert.Single(errorDict);
        Assert.True(errorDict.ContainsKey(""));
        Assert.Equal(2, errorDict[""].Count);
        Assert.Contains(errors[0].Description, errorDict[""]);
        Assert.Contains(errors[1].Description, errorDict[""]);
    }

    [Fact]
    public void MapIdentityErrorsToFields_WithMixedErrors_GroupsByField()
    {
        // Arrange
        var errors = new[]
        {
            _errorDescriber.DuplicateUserName("testuser"),
            _errorDescriber.PasswordTooShort(8),
            _errorDescriber.InvalidUserName("test@user"),
            _errorDescriber.PasswordRequiresDigit(),
            _errorDescriber.DefaultError()
        };
        var identityResult = IdentityResult.Failed(errors);
        var extendedResult = new ExtendedIdentityResult<IdentityUser>(identityResult, null);

        // Act
        var errorDict = extendedResult.MapIdentityErrorsToFields(
            usernameField: "Username",
            passwordField: "Pass",
            confirmPasswordField: "ConfirmPass"
        );

        // Assert
        Assert.Equal(3, errorDict.Count);
        Assert.True(errorDict.ContainsKey("Username"));
        Assert.True(errorDict.ContainsKey("Pass"));
        Assert.True(errorDict.ContainsKey(""));
        Assert.Equal(2, errorDict["Username"].Count);
        Assert.Equal(2, errorDict["Pass"].Count);
        Assert.Single(errorDict[""]);
    }

    [Fact]
    public void MapIdentityErrorsToFields_WithEmptyFieldNames_StillMapsCorrectly()
    {
        // Arrange
        var errors = new[]
        {
            _errorDescriber.DuplicateUserName("testuser"),
            _errorDescriber.PasswordTooShort(6)
        };
        var identityResult = IdentityResult.Failed(errors);
        var extendedResult = new ExtendedIdentityResult<IdentityUser>(identityResult, null);

        // Act
        var errorDict = extendedResult.MapIdentityErrorsToFields();

        // Assert
        Assert.Single(errorDict);
        Assert.True(errorDict.ContainsKey(""));
        Assert.Equal(2, errorDict[""].Count);
    }

    [Fact]
    public void MapIdentityErrorsToFields_WithNoErrors_ReturnsEmptyDictionary()
    {
        // Arrange
        var identityResult = IdentityResult.Success;
        var extendedResult = new ExtendedIdentityResult<IdentityUser>(identityResult, null);

        // Act
        var errorDict = extendedResult.MapIdentityErrorsToFields(
            usernameField: "Username",
            passwordField: "Password"
        );

        // Assert
        Assert.Empty(errorDict);
    }

    #endregion

    #region Factory Methods Tests

    [Fact]
    public void Success_CreatesSuccessfulResult()
    {
        // Arrange
        var user = new IdentityUser { UserName = "testuser" };

        // Act
        var result = ExtendedIdentityResult<IdentityUser>.Success(user);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Value);
        Assert.Equal("testuser", result.Value.UserName);
        Assert.True(result.IdentityResult.Succeeded);
    }

    [Fact]
    public void Failure_CreatesFailedResultWithoutValue()
    {
        // Arrange
        var errors = new[]
        {
            new IdentityError { Code = "TestError", Description = "Test error description." }
        };
        var identityResult = IdentityResult.Failed(errors);

        // Act
        var result = ExtendedIdentityResult<IdentityUser>.Failure(identityResult);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Null(result.Value);
        Assert.False(result.IdentityResult.Succeeded);
        Assert.Single(result.IdentityResult.Errors);
        Assert.Equal("TestError", result.IdentityResult.Errors.First().Code);
    }

    #endregion

    #region Properties Tests

    [Fact]
    public void Succeeded_ReturnsTrueForSuccessfulResult()
    {
        // Arrange
        var user = new IdentityUser();
        var result = new ExtendedIdentityResult<IdentityUser>(IdentityResult.Success, user);

        // Act & Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Succeeded_ReturnsFalseForFailedResult()
    {
        // Arrange
        var errors = new[] { new IdentityError { Code = "Error", Description = "Description" } };
        var identityResult = IdentityResult.Failed(errors);
        var result = new ExtendedIdentityResult<IdentityUser>(identityResult, null);

        // Act & Assert
        Assert.False(result.Succeeded);
    }

    [Fact]
    public void Value_ReturnsNullForFailedResult()
    {
        // Arrange
        var errors = new[] { new IdentityError { Code = "Error", Description = "Description" } };
        var identityResult = IdentityResult.Failed(errors);
        var result = new ExtendedIdentityResult<IdentityUser>(identityResult, null);

        // Act & Assert
        Assert.Null(result.Value);
    }

    #endregion
}
