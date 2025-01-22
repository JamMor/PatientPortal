using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for invalid input (e.g., null/empty descriptions, missing patientId).
// TODO: Add tests ensuring patientId is honored during delete operations (should not delete unrelated patient issues).
// TODO: Add boundary tests (very long descriptions, maximum counts) and empty-database behaviors.
public class HealthIssueServiceTests : IDisposable
{
    private readonly PatientPortalContext _context;
    private readonly HealthIssueService _healthIssueService;

    public HealthIssueServiceTests()
    {
        var options = new DbContextOptionsBuilder<PatientPortalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientPortalContext(options);
        _healthIssueService = new HealthIssueService(_context);
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

    private void AddHealthIssues(params HealthIssue[] issues)
    {
        _context.HealthIssues.AddRange(issues);
        _context.SaveChanges();
    }

    #endregion

    #region CreateHealthIssue Tests

    [Fact]
    public void CreateHealthIssue_WithValidData_CreatesHealthIssue()
    {
        // Arrange
        var patient = CreatePatient();
        var healthIssue = new HealthIssue
        {
            ShortDescription = "Headache",
            LongDescription = "Recurring headaches for the past week"
        };

        // Act
        _healthIssueService.CreateHealthIssue(patient.PatientId, healthIssue);

        // Assert
        var savedIssue = _context.HealthIssues.FirstOrDefault();
        Assert.NotNull(savedIssue);
        Assert.Equal("Headache", savedIssue.ShortDescription);
        Assert.Equal("Recurring headaches for the past week", savedIssue.LongDescription);
        Assert.Equal(patient.PatientId, savedIssue.PatientId);
    }

    [Fact]
    public void CreateHealthIssue_SetsPatientIdCorrectly()
    {
        // Arrange
        var patient = CreatePatient();
        var healthIssue = new HealthIssue
        {
            ShortDescription = "Back Pain",
            LongDescription = "Lower back pain"
        };

        // Act
        _healthIssueService.CreateHealthIssue(patient.PatientId, healthIssue);

        // Assert
        var savedIssue = _context.HealthIssues.FirstOrDefault();
        Assert.NotNull(savedIssue);
        Assert.Equal(patient.PatientId, savedIssue.PatientId);
    }

    [Fact]
    public void CreateHealthIssue_MultipleIssuesForSamePatient_AllCreated()
    {
        // Arrange
        var patient = CreatePatient();
        var issue1 = new HealthIssue { ShortDescription = "Issue 1", LongDescription = "Description 1" };
        var issue2 = new HealthIssue { ShortDescription = "Issue 2", LongDescription = "Description 2" };
        var issue3 = new HealthIssue { ShortDescription = "Issue 3", LongDescription = "Description 3" };

        // Act
        _healthIssueService.CreateHealthIssue(patient.PatientId, issue1);
        _healthIssueService.CreateHealthIssue(patient.PatientId, issue2);
        _healthIssueService.CreateHealthIssue(patient.PatientId, issue3);

        // Assert
        var patientIssues = _context.HealthIssues.Where(h => h.PatientId == patient.PatientId).ToList();
        Assert.Equal(3, patientIssues.Count);
    }

    [Fact]
    public void CreateHealthIssue_WithMinimalData_CreatesSuccessfully()
    {
        // Arrange
        var patient = CreatePatient();
        var healthIssue = new HealthIssue
        {
            ShortDescription = "Minor Issue"
            // LongDescription is optional
        };

        // Act
        _healthIssueService.CreateHealthIssue(patient.PatientId, healthIssue);

        // Assert
        var savedIssue = _context.HealthIssues.FirstOrDefault();
        Assert.NotNull(savedIssue);
        Assert.Equal("Minor Issue", savedIssue.ShortDescription);
    }

    [Fact]
    public void CreateHealthIssue_GeneratesHealthIssueId()
    {
        // Arrange
        var patient = CreatePatient();
        var healthIssue = new HealthIssue
        {
            ShortDescription = "Test Issue",
            LongDescription = "Test Description"
        };

        // Act
        _healthIssueService.CreateHealthIssue(patient.PatientId, healthIssue);

        // Assert
        var savedIssue = _context.HealthIssues.FirstOrDefault();
        Assert.NotNull(savedIssue);
        Assert.True(savedIssue.HealthIssueId > 0);
    }

    #endregion

    #region DeleteHealthIssue Tests

    [Fact]
    public void DeleteHealthIssue_WithExistingIssue_RemovesIssue()
    {
        // Arrange
        var patient = CreatePatient();
        var healthIssue = new HealthIssue
        {
            PatientId = patient.PatientId,
            ShortDescription = "To Delete",
            LongDescription = "This will be deleted"
        };
        AddHealthIssues(healthIssue);
        var issueId = healthIssue.HealthIssueId;

        // Act
        _healthIssueService.DeleteHealthIssue(patient.PatientId, issueId);

        // Assert
        var deletedIssue = _context.HealthIssues.Find(issueId);
        Assert.Null(deletedIssue);
    }

    [Fact]
    public void DeleteHealthIssue_WithNonExistingIssue_DoesNotThrow()
    {
        // Arrange
        var patient = CreatePatient();
        var nonExistentIssueId = 99999;

        // Act & Assert
        var exception = Record.Exception(() => 
            _healthIssueService.DeleteHealthIssue(patient.PatientId, nonExistentIssueId));
        Assert.Null(exception);
    }

    [Fact]
    public void DeleteHealthIssue_OnlyDeletesSpecifiedIssue()
    {
        // Arrange
        var patient = CreatePatient();
        var issue1 = new HealthIssue { PatientId = patient.PatientId, ShortDescription = "Issue 1" };
        var issue2 = new HealthIssue { PatientId = patient.PatientId, ShortDescription = "Issue 2" };
        AddHealthIssues(issue1, issue2);

        // Act
        _healthIssueService.DeleteHealthIssue(patient.PatientId, issue1.HealthIssueId);

        // Assert
        Assert.Single(_context.HealthIssues);
        Assert.Equal("Issue 2", _context.HealthIssues.First().ShortDescription);
    }

    [Fact]
    public void DeleteHealthIssue_WithDifferentPatientId_StillDeletesIfIssueExists()
    {
        // Arrange - Note: The service only uses issueId for deletion, patientId is not used in the query
        var patient1 = CreatePatient("John", "Doe");
        var patient2 = CreatePatient("Jane", "Smith");
        var healthIssue = new HealthIssue
        {
            PatientId = patient1.PatientId,
            ShortDescription = "Patient 1's Issue"
        };
        AddHealthIssues(healthIssue);

        // Act - Note: Current implementation only uses issueId
        _healthIssueService.DeleteHealthIssue(patient2.PatientId, healthIssue.HealthIssueId);

        // TODO: Confirm whether deletion should enforce patientId; current implementation ignores it.
        // Assert - Issue is deleted because only issueId is checked
        var deletedIssue = _context.HealthIssues.Find(healthIssue.HealthIssueId);
        Assert.Null(deletedIssue);
    }

    #endregion

    public void Dispose()
    {
        _healthIssueService?.Dispose();
        _context?.Dispose();
    }
}
