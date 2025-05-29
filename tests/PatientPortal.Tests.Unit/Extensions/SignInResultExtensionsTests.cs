using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PatientPortal.Extensions;

namespace PatientPortal.Tests.Unit.Extensions;

// Tests for SignInResult extension methods that add appropriate error messages to ModelState
public class SignInResultExtensionsTests
{
    #region AddErrorToModelState Tests

    [Fact]
    public void AddErrorToModelState_WithLockedOutResult_AddsLockoutMessage()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        var result = SignInResult.LockedOut;

        // Act
        result.AddErrorToModelState(modelState);

        // Assert
        Assert.False(modelState.IsValid);
        Assert.Single(modelState);
        Assert.True(modelState.ContainsKey(string.Empty));
        var errors = modelState[string.Empty]!.Errors;
        Assert.Single(errors);
        Assert.Equal("Account is locked due to multiple failed login attempts. Please try again later.", errors[0].ErrorMessage);
    }

    [Fact]
    public void AddErrorToModelState_WithNotAllowedResult_AddsNotAllowedMessage()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        var result = SignInResult.NotAllowed;

        // Act
        result.AddErrorToModelState(modelState);

        // Assert
        Assert.False(modelState.IsValid);
        Assert.Single(modelState);
        Assert.True(modelState.ContainsKey(string.Empty));
        var errors = modelState[string.Empty]!.Errors;
        Assert.Single(errors);
        Assert.Equal("Login is not allowed for this account.", errors[0].ErrorMessage);
    }

    [Fact]
    public void AddErrorToModelState_WithTwoFactorRequiredResult_AddsTwoFactorMessage()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        var result = SignInResult.TwoFactorRequired;

        // Act
        result.AddErrorToModelState(modelState);

        // Assert
        Assert.False(modelState.IsValid);
        Assert.Single(modelState);
        Assert.True(modelState.ContainsKey(string.Empty));
        var errors = modelState[string.Empty]!.Errors;
        Assert.Single(errors);
        Assert.Equal("Two-factor authentication is required.", errors[0].ErrorMessage);
    }

    [Fact]
    public void AddErrorToModelState_WithFailedResult_AddsInvalidCredentialsMessage()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        var result = SignInResult.Failed;

        // Act
        result.AddErrorToModelState(modelState);

        // Assert
        Assert.False(modelState.IsValid);
        Assert.Single(modelState);
        Assert.True(modelState.ContainsKey(string.Empty));
        var errors = modelState[string.Empty]!.Errors;
        Assert.Single(errors);
        Assert.Equal("Invalid credentials.", errors[0].ErrorMessage);
    }

    [Fact]
    public void AddErrorToModelState_DoesNotAddDuplicateErrors()
    {
        // Arrange
        var modelState = new ModelStateDictionary();
        var result = SignInResult.LockedOut;

        // Act
        result.AddErrorToModelState(modelState);

        // Assert - Verify only one error added, not multiple
        Assert.False(modelState.IsValid);
        Assert.Single(modelState);
        var errors = modelState[string.Empty]!.Errors;
        Assert.Single(errors);
    }

    #endregion
}
