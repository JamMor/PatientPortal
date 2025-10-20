using Microsoft.EntityFrameworkCore;
using PatientPortal.DTOs;
using PatientPortal.Models;
using PatientPortal.Services;
using TestResultModel = PatientPortal.Models.TestResult;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for invalid input (e.g., null form data, missing TestResult, invalid patient/staff IDs).
// TODO: Add boundary tests for large comments/types and empty-database scenarios.
// TODO: Add tests for health issue associations when IDs do not belong to the patient.
public class TestResultServiceTests : IDisposable
{
    private readonly PatientPortalContext _context;
    private readonly TestResultService _testResultService;

    public TestResultServiceTests()
    {
        var options = new DbContextOptionsBuilder<PatientPortalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientPortalContext(options);
        _testResultService = new TestResultService(_context);
    }

    #region Helper Methods

    private Patient CreatePatient(string firstName = "John", string lastName = "Doe")
    {
        var patient = new Patient
        {
            FirstName = firstName,
            LastName = lastName,
            DOB = new DateTime(1990, 1, 1),
            Last4SSN = "1234",
            MessagingLink = new MessagingLink()
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();
        return patient;
    }

    private Staff CreateStaff(string firstName = "Dr", string lastName = "Smith")
    {
        var staff = new Staff
        {
            FirstName = firstName,
            LastName = lastName,
            StaffUsername = $"{firstName.ToLower()}{lastName.ToLower()}",
            Password = "hashedpassword",
            Role = "Doctor",
            IsAdmin = false,
            MessagingLink = new MessagingLink()
        };
        _context.Staff.Add(staff);
        _context.SaveChanges();
        return staff;
    }

    private HealthIssue CreateHealthIssue(int patientId, string description = "Test Issue")
    {
        var healthIssue = new HealthIssue
        {
            PatientId = patientId,
            ShortDescription = description,
            LongDescription = "Detailed description"
        };
        _context.HealthIssues.Add(healthIssue);
        _context.SaveChanges();
        return healthIssue;
    }

    private void AddTestResults(params TestResultModel[] results)
    {
        _context.TestResults.AddRange(results);
        _context.SaveChanges();
    }

    #endregion

    #region CreateTestResult Tests

    [Fact]
    public void CreateTestResult_WithValidData_CreatesTestResult()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();

        var formData = new TestResultDTO("Blood Test", "Normal levels", []);

        // Act
        _testResultService.CreateTestResult(patient.PatientId, staff.StaffId, formData);

        // Assert
        var savedResult = _context.TestResults.FirstOrDefault();
        Assert.NotNull(savedResult);
        Assert.Equal("Blood Test", savedResult.Type);
        Assert.Equal("Normal levels", savedResult.Comment);
        Assert.Equal(patient.PatientId, savedResult.PatientId);
        Assert.Equal(staff.StaffId, savedResult.StaffId);
    }

    [Fact]
    public void CreateTestResult_WithHealthIssues_AssociatesHealthIssues()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();
        var healthIssue1 = CreateHealthIssue(patient.PatientId, "Issue 1");
        var healthIssue2 = CreateHealthIssue(patient.PatientId, "Issue 2");

        var formData = new TestResultDTO(
            "MRI Scan",
            "Scan results",
            [healthIssue1.HealthIssueId, healthIssue2.HealthIssueId]
        );

        // Act
        _testResultService.CreateTestResult(patient.PatientId, staff.StaffId, formData);

        // Assert
        var savedResult = _context.TestResults
            .Include(t => t.AssociatedHealthIssues)
            .FirstOrDefault();
        Assert.NotNull(savedResult);
        Assert.Equal(2, savedResult.AssociatedHealthIssues.Count);
    }

    [Fact]
    public void CreateTestResult_WithUnselectedHealthIssues_OnlyAssociatesSelected()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();
        var healthIssue1 = CreateHealthIssue(patient.PatientId, "Selected Issue");
        var healthIssue2 = CreateHealthIssue(patient.PatientId, "Unselected Issue");

        var formData = new TestResultDTO(
            "X-Ray",
            "X-Ray results",
            [healthIssue1.HealthIssueId]
        );

        // Act
        _testResultService.CreateTestResult(patient.PatientId, staff.StaffId, formData);

        // Assert
        var savedResult = _context.TestResults
            .Include(t => t.AssociatedHealthIssues)
            .FirstOrDefault();
        Assert.NotNull(savedResult);
        Assert.Single(savedResult.AssociatedHealthIssues);
        Assert.Equal(healthIssue1.HealthIssueId, savedResult.AssociatedHealthIssues.First().HealthIssueId);
    }

    [Fact]
    public void CreateTestResult_WithNoHealthIssues_CreatesWithoutAssociations()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();

        var formData = new TestResultDTO("Routine Check", "All normal", []);

        // Act
        _testResultService.CreateTestResult(patient.PatientId, staff.StaffId, formData);

        // Assert
        var savedResult = _context.TestResults
            .Include(t => t.AssociatedHealthIssues)
            .FirstOrDefault();
        Assert.NotNull(savedResult);
        Assert.Empty(savedResult.AssociatedHealthIssues);
    }

    [Fact]
    public void CreateTestResult_GeneratesTestResultId()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();

        var formData = new TestResultDTO("Lab Test", "Results pending", []);

        // Act
        _testResultService.CreateTestResult(patient.PatientId, staff.StaffId, formData);

        // Assert
        var savedResult = _context.TestResults.FirstOrDefault();
        Assert.NotNull(savedResult);
        Assert.True(savedResult.TestResultId > 0);
    }

    [Fact]
    public void CreateTestResult_MultipleResultsForSamePatient_AllCreated()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();

        var formData1 = new TestResultDTO("Test 1", "Comment 1", []);
        var formData2 = new TestResultDTO("Test 2", "Comment 2", []);

        // Act
        _testResultService.CreateTestResult(patient.PatientId, staff.StaffId, formData1);
        _testResultService.CreateTestResult(patient.PatientId, staff.StaffId, formData2);

        // Assert
        var results = _context.TestResults.Where(t => t.PatientId == patient.PatientId).ToList();
        Assert.Equal(2, results.Count);
    }

    #endregion

    #region DeleteTestResult Tests

    [Fact]
    public void DeleteTestResult_WithExistingResult_RemovesResult()
    {
        // Arrange
        var testResult = new TestResultModel
        {
            Type = "To Delete",
            Comment = "This will be deleted",
            PatientId = 1,
            StaffId = 1
        };
        AddTestResults(testResult);
        var resultId = testResult.TestResultId;

        // Act
        _testResultService.DeleteTestResult(resultId);

        // Assert
        var deletedResult = _context.TestResults.Find(resultId);
        Assert.Null(deletedResult);
    }

    [Fact]
    public void DeleteTestResult_WithNonExistingResult_DoesNotThrow()
    {
        // Arrange
        var nonExistentResultId = 99999;

        // Act & Assert
        var exception = Record.Exception(() => _testResultService.DeleteTestResult(nonExistentResultId));
        Assert.Null(exception);
    }

    [Fact]
    public void DeleteTestResult_OnlyDeletesSpecifiedResult()
    {
        // Arrange
        var result1 = new TestResultModel { Type = "Test 1", Comment = "Comment 1", PatientId = 1, StaffId = 1 };
        var result2 = new TestResultModel { Type = "Test 2", Comment = "Comment 2", PatientId = 1, StaffId = 1 };
        AddTestResults(result1, result2);

        // Act
        _testResultService.DeleteTestResult(result1.TestResultId);

        // Assert
        Assert.Single(_context.TestResults);
        Assert.Equal("Test 2", _context.TestResults.First().Type);
    }

    [Fact]
    public void DeleteTestResult_WithAssociatedHealthIssues_DeletesResult()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();
        var healthIssue = CreateHealthIssue(patient.PatientId);

        var testResult = new TestResultModel
        {
            Type = "With Associations",
            Comment = "Has health issue associations",
            PatientId = patient.PatientId,
            StaffId = staff.StaffId,
            AssociatedHealthIssues = new List<TestHealthIssueAssociation>
            {
                new TestHealthIssueAssociation { HealthIssueId = healthIssue.HealthIssueId }
            }
        };
        AddTestResults(testResult);

        // Act
        _testResultService.DeleteTestResult(testResult.TestResultId);

        // Assert
        var deletedResult = _context.TestResults.Find(testResult.TestResultId);
        Assert.Null(deletedResult);
    }

    #endregion

    public void Dispose()
    {
        _context?.Dispose();
    }
}
