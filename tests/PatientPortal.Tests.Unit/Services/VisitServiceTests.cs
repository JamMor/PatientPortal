using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for invalid input (e.g., null VisitFormView, missing Visit, missing patient/staff IDs).
// TODO: Add boundary tests for dates (far past/future) and comment length constraints.
// TODO: Add tests for behavior when patient/staff does not exist (should fail or no-op as intended).
public class VisitServiceTests : IDisposable
{
    private readonly PatientPortalContext _context;
    private readonly VisitService _visitService;

    public VisitServiceTests()
    {
        // Create in-memory database for testing
        var options = new DbContextOptionsBuilder<PatientPortalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientPortalContext(options);
        _visitService = new VisitService(_context);
    }

    #region Helper Methods

    private Patient CreatePatient(string firstName = "John", string lastName = "Doe", string last4Ssn = "1234")
    {
        var patient = new Patient
        {
            FirstName = firstName,
            LastName = lastName,
            DOB = new DateTime(1990, 1, 1),
            Last4SSN = last4Ssn,
            MessagingLink = new MessagingLink()
        };
        _context.Patients.Add(patient);
        _context.SaveChanges();
        return patient;
    }

    private Staff CreateStaff(string firstName = "Dr", string lastName = "Smith", string username = "drsmith", string role = "Doctor")
    {
        var staff = new Staff
        {
            FirstName = firstName,
            LastName = lastName,
            StaffUsername = username,
            Password = "hashedpassword",
            Role = role,
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
            LongDescription = "Test Description"
        };
        _context.HealthIssues.Add(healthIssue);
        _context.SaveChanges();
        return healthIssue;
    }

    #endregion

    #region CreateVisit Tests

    [Fact]
    public void CreateVisit_WithValidData_CreatesVisit()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();
        var healthIssue = CreateHealthIssue(patient.PatientId);

        var visitFormView = new VisitFormView
        {
            Visit = new Visit
            {
                Comment = "Regular checkup",
                DateOfVisit = DateTime.Today
            },
            HealthIssues = new List<HealthIssueCheckbox>
            {
                new HealthIssueCheckbox { HealthIssueId = healthIssue.HealthIssueId, Selected = true }
            }
        };

        // Act
        _visitService.CreateVisit(patient.PatientId, staff.StaffId, visitFormView);

        // Assert
        var createdVisit = _context.Visits
            .Include(v => v.AssociatedHealthIssues)
            .FirstOrDefault();
        
        Assert.NotNull(createdVisit);
        Assert.Equal("Regular checkup", createdVisit.Comment);
        Assert.Equal(DateTime.Today, createdVisit.DateOfVisit);
        Assert.Equal(patient.PatientId, createdVisit.PatientId);
        Assert.Equal(staff.StaffId, createdVisit.StaffId);
        Assert.Single(createdVisit.AssociatedHealthIssues);
        Assert.Equal(healthIssue.HealthIssueId, createdVisit.AssociatedHealthIssues.First().HealthIssueId);
    }

    [Fact]
    public void CreateVisit_WithNoSelectedHealthIssues_CreatesVisitWithoutHealthIssues()
    {
        // Arrange
        var patient = CreatePatient("Jane", "Smith", "5678");
        var staff = CreateStaff("Dr", "Johnson", "drjohnson");

        var visitFormView = new VisitFormView
        {
            Visit = new Visit
            {
                Comment = "Emergency visit",
                DateOfVisit = DateTime.Today.AddDays(-1)
            },
            HealthIssues = new List<HealthIssueCheckbox>()
        };

        // Act
        _visitService.CreateVisit(patient.PatientId, staff.StaffId, visitFormView);

        // Assert
        var createdVisit = _context.Visits
            .Include(v => v.AssociatedHealthIssues)
            .FirstOrDefault();
        
        Assert.NotNull(createdVisit);
        Assert.Equal("Emergency visit", createdVisit.Comment);
        Assert.Equal(DateTime.Today.AddDays(-1), createdVisit.DateOfVisit);
        Assert.Empty(createdVisit.AssociatedHealthIssues);
    }

    [Fact]
    public void CreateVisit_WithMultipleHealthIssues_AssociatesAll()
    {
        // Arrange
        var patient = CreatePatient("Multi", "Issue", "9999");
        var staff = CreateStaff("Dr", "Multi", "drmulti");

        var healthIssue1 = CreateHealthIssue(patient.PatientId, "Issue 1");
        var healthIssue2 = CreateHealthIssue(patient.PatientId, "Issue 2");
        var healthIssue3 = CreateHealthIssue(patient.PatientId, "Issue 3");

        var visitFormView = new VisitFormView
        {
            Visit = new Visit { Comment = "Multi-issue visit", DateOfVisit = DateTime.Today },
            HealthIssues = new List<HealthIssueCheckbox>
            {
                new HealthIssueCheckbox { HealthIssueId = healthIssue1.HealthIssueId, Selected = true },
                new HealthIssueCheckbox { HealthIssueId = healthIssue2.HealthIssueId, Selected = true },
                new HealthIssueCheckbox { HealthIssueId = healthIssue3.HealthIssueId, Selected = true }
            }
        };

        // Act
        _visitService.CreateVisit(patient.PatientId, staff.StaffId, visitFormView);

        // Assert
        var createdVisit = _context.Visits
            .Include(v => v.AssociatedHealthIssues)
            .FirstOrDefault();
        Assert.NotNull(createdVisit);
        Assert.Equal(3, createdVisit.AssociatedHealthIssues.Count);
    }

    [Fact]
    public void CreateVisit_WithMixedSelectedHealthIssues_OnlyAssociatesSelected()
    {
        // Arrange
        var patient = CreatePatient("Mixed", "Selection", "8888");
        var staff = CreateStaff("Dr", "Mixed", "drmixed");

        var healthIssue1 = CreateHealthIssue(patient.PatientId, "Selected 1");
        var healthIssue2 = CreateHealthIssue(patient.PatientId, "Not Selected");
        var healthIssue3 = CreateHealthIssue(patient.PatientId, "Selected 2");

        var visitFormView = new VisitFormView
        {
            Visit = new Visit { Comment = "Mixed selection visit", DateOfVisit = DateTime.Today },
            HealthIssues = new List<HealthIssueCheckbox>
            {
                new HealthIssueCheckbox { HealthIssueId = healthIssue1.HealthIssueId, Selected = true },
                new HealthIssueCheckbox { HealthIssueId = healthIssue2.HealthIssueId, Selected = false },
                new HealthIssueCheckbox { HealthIssueId = healthIssue3.HealthIssueId, Selected = true }
            }
        };

        // Act
        _visitService.CreateVisit(patient.PatientId, staff.StaffId, visitFormView);

        // Assert
        var createdVisit = _context.Visits
            .Include(v => v.AssociatedHealthIssues)
            .FirstOrDefault();
        Assert.NotNull(createdVisit);
        Assert.Equal(2, createdVisit.AssociatedHealthIssues.Count);
    }

    [Fact]
    public void CreateVisit_GeneratesVisitId()
    {
        // Arrange
        var patient = CreatePatient("ID", "Test", "1111");
        var staff = CreateStaff("Dr", "ID", "drid");

        var visitFormView = new VisitFormView
        {
            Visit = new Visit { Comment = "ID test", DateOfVisit = DateTime.Today },
            HealthIssues = new List<HealthIssueCheckbox>()
        };

        // Act
        _visitService.CreateVisit(patient.PatientId, staff.StaffId, visitFormView);

        // Assert
        var createdVisit = _context.Visits.FirstOrDefault();
        Assert.NotNull(createdVisit);
        Assert.True(createdVisit.VisitId > 0);
    }

    [Fact]
    public void CreateVisit_SetsPatientIdAndStaffIdCorrectly()
    {
        // Arrange
        var patient = CreatePatient("Association", "Test", "6666");
        var staff = CreateStaff("Dr", "Association", "drassoc");

        var visitFormView = new VisitFormView
        {
            Visit = new Visit { Comment = "Association test", DateOfVisit = DateTime.Today },
            HealthIssues = new List<HealthIssueCheckbox>()
        };

        // Act
        _visitService.CreateVisit(patient.PatientId, staff.StaffId, visitFormView);

        // Assert
        var createdVisit = _context.Visits.FirstOrDefault();
        Assert.NotNull(createdVisit);
        Assert.Equal(patient.PatientId, createdVisit.PatientId);
        Assert.Equal(staff.StaffId, createdVisit.StaffId);
    }

    [Fact]
    public void CreateVisit_WithPastDate_CreatesVisitWithPastDate()
    {
        // Arrange
        var patient = CreatePatient("Past", "Date", "5555");
        var staff = CreateStaff("Dr", "Past", "drpast");

        var pastDate = DateTime.Today.AddDays(-30);
        var visitFormView = new VisitFormView
        {
            Visit = new Visit { Comment = "Past date visit", DateOfVisit = pastDate },
            HealthIssues = new List<HealthIssueCheckbox>()
        };

        // Act
        _visitService.CreateVisit(patient.PatientId, staff.StaffId, visitFormView);

        // Assert
        var createdVisit = _context.Visits.FirstOrDefault();
        Assert.NotNull(createdVisit);
        Assert.Equal(pastDate, createdVisit.DateOfVisit);
    }

    [Fact]
    public void CreateVisit_MultipleVisitsForSamePatient_AllCreated()
    {
        // Arrange
        var patient = CreatePatient("Multiple", "Visits", "4444");
        var staff = CreateStaff("Dr", "Multiple", "drmultiple");

        var visitFormView1 = new VisitFormView
        {
            Visit = new Visit { Comment = "Visit 1", DateOfVisit = DateTime.Today.AddDays(-10) },
            HealthIssues = new List<HealthIssueCheckbox>()
        };
        var visitFormView2 = new VisitFormView
        {
            Visit = new Visit { Comment = "Visit 2", DateOfVisit = DateTime.Today.AddDays(-5) },
            HealthIssues = new List<HealthIssueCheckbox>()
        };
        var visitFormView3 = new VisitFormView
        {
            Visit = new Visit { Comment = "Visit 3", DateOfVisit = DateTime.Today },
            HealthIssues = new List<HealthIssueCheckbox>()
        };

        // Act
        _visitService.CreateVisit(patient.PatientId, staff.StaffId, visitFormView1);
        _visitService.CreateVisit(patient.PatientId, staff.StaffId, visitFormView2);
        _visitService.CreateVisit(patient.PatientId, staff.StaffId, visitFormView3);

        // Assert
        var visits = _context.Visits.Where(v => v.PatientId == patient.PatientId).ToList();
        Assert.Equal(3, visits.Count);
    }

    #endregion

    #region DeleteVisit Tests

    [Fact]
    public void DeleteVisit_WithExistingVisit_RemovesVisit()
    {
        // Arrange
        var visit = new Visit
        {
            Comment = "Test visit",
            DateOfVisit = DateTime.Today,
            PatientId = 1,
            StaffId = 1
        };
        _context.Visits.Add(visit);
        _context.SaveChanges();
        var visitId = visit.VisitId;

        // Act
        _visitService.DeleteVisit(visitId);

        // Assert
        var deletedVisit = _context.Visits.Find(visitId);
        Assert.Null(deletedVisit);
    }

    [Fact]
    public void DeleteVisit_WithNonExistingVisit_DoesNotThrow()
    {
        // Arrange
        var nonExistingVisitId = 99999;

        // Act & Assert
        var exception = Record.Exception(() => _visitService.DeleteVisit(nonExistingVisitId));
        Assert.Null(exception);
    }

    [Fact]
    public void DeleteVisit_OnlyDeletesSpecifiedVisit()
    {
        // Arrange
        var visit1 = new Visit { Comment = "Visit 1", DateOfVisit = DateTime.Today, PatientId = 1, StaffId = 1 };
        var visit2 = new Visit { Comment = "Visit 2", DateOfVisit = DateTime.Today, PatientId = 1, StaffId = 1 };
        _context.Visits.AddRange(visit1, visit2);
        _context.SaveChanges();

        // Act
        _visitService.DeleteVisit(visit1.VisitId);

        // Assert
        Assert.Single(_context.Visits);
        Assert.Equal("Visit 2", _context.Visits.First().Comment);
    }

    #endregion

    public void Dispose()
    {
        _visitService?.Dispose();
        _context?.Dispose();
    }
}
