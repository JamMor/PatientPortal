using Microsoft.EntityFrameworkCore;
using PatientPortal.Models;
using PatientPortal.Services;

namespace PatientPortal.Tests.Unit.Services;

// TODO: Add tests for invalid IDs (negative/zero) and null handling on add/remove operations.
// TODO: Add tests ensuring add/remove operations enforce patient/staff existence (or document current orphaning behavior).
// TODO: Add boundary tests for large team sizes and duplicate prevention when concurrent requests occur.
public class PatientStaffConnectionServiceTests : IDisposable
{
    private readonly PatientPortalContext _context;
    private readonly PatientStaffConnectionService _connectionService;

    public PatientStaffConnectionServiceTests()
    {
        var options = new DbContextOptionsBuilder<PatientPortalContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PatientPortalContext(options);
        _connectionService = new PatientStaffConnectionService(_context);
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

    private Staff CreateStaff(string firstName = "Dr", string lastName = "Smith", string role = "Doctor")
    {
        var staff = new Staff
        {
            FirstName = firstName,
            LastName = lastName,
            StaffUsername = $"{firstName.ToLower()}{lastName.ToLower()}{Guid.NewGuid().ToString().Substring(0, 4)}",
            Password = "hashedpassword",
            Role = role,
            IsAdmin = false,
            MessagingLink = new MessagingLink()
        };
        _context.Staff.Add(staff);
        _context.SaveChanges();
        return staff;
    }

    #endregion

    #region AddStaffToPatientTeam Tests

    [Fact]
    public void AddStaffToPatientTeam_WithValidIds_CreatesConnection()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();

        // Act
        _connectionService.AddStaffToPatientTeam(patient.PatientId, staff.StaffId);

        // Assert
        var connection = _context.PatientStaffConnections
            .FirstOrDefault(c => c.PatientId == patient.PatientId && c.StaffId == staff.StaffId);
        Assert.NotNull(connection);
    }

    [Fact]
    public void AddStaffToPatientTeam_DuplicateConnection_DoesNotCreateDuplicate()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();

        // Act - Add same connection twice
        _connectionService.AddStaffToPatientTeam(patient.PatientId, staff.StaffId);
        _connectionService.AddStaffToPatientTeam(patient.PatientId, staff.StaffId);

        // Assert
        var connections = _context.PatientStaffConnections
            .Where(c => c.PatientId == patient.PatientId && c.StaffId == staff.StaffId)
            .ToList();
        Assert.Single(connections);
    }

    [Fact]
    public void AddStaffToPatientTeam_MultipleStaffToSamePatient_CreatesAllConnections()
    {
        // Arrange
        var patient = CreatePatient();
        var doctor = CreateStaff("Dr", "Smith", "Doctor");
        var nurse = CreateStaff("Jane", "Nurse", "Nurse");
        var therapist = CreateStaff("Bob", "Therapist", "Physical Therapist");

        // Act
        _connectionService.AddStaffToPatientTeam(patient.PatientId, doctor.StaffId);
        _connectionService.AddStaffToPatientTeam(patient.PatientId, nurse.StaffId);
        _connectionService.AddStaffToPatientTeam(patient.PatientId, therapist.StaffId);

        // Assert
        var patientTeam = _context.PatientStaffConnections
            .Where(c => c.PatientId == patient.PatientId)
            .ToList();
        Assert.Equal(3, patientTeam.Count);
    }

    [Fact]
    public void AddStaffToPatientTeam_SameStaffToMultiplePatients_CreatesAllConnections()
    {
        // Arrange
        var patient1 = CreatePatient("John", "Doe");
        var patient2 = CreatePatient("Jane", "Smith");
        var patient3 = CreatePatient("Bob", "Wilson");
        var doctor = CreateStaff();

        // Act
        _connectionService.AddStaffToPatientTeam(patient1.PatientId, doctor.StaffId);
        _connectionService.AddStaffToPatientTeam(patient2.PatientId, doctor.StaffId);
        _connectionService.AddStaffToPatientTeam(patient3.PatientId, doctor.StaffId);

        // Assert
        var doctorPatients = _context.PatientStaffConnections
            .Where(c => c.StaffId == doctor.StaffId)
            .ToList();
        Assert.Equal(3, doctorPatients.Count);
    }

    [Fact]
    public void AddStaffToPatientTeam_WithNonExistentPatient_DoesNotThrow()
    {
        // Arrange
        var staff = CreateStaff();
        var nonExistentPatientId = 99999;

        // Act & Assert - Should not throw, just creates orphan connection
        var exception = Record.Exception(() => 
            _connectionService.AddStaffToPatientTeam(nonExistentPatientId, staff.StaffId));
        Assert.Null(exception);
    }

    [Fact]
    public void AddStaffToPatientTeam_WithNonExistentStaff_DoesNotThrow()
    {
        // Arrange
        var patient = CreatePatient();
        var nonExistentStaffId = 99999;

        // Act & Assert - Should not throw, just creates orphan connection
        var exception = Record.Exception(() => 
            _connectionService.AddStaffToPatientTeam(patient.PatientId, nonExistentStaffId));
        Assert.Null(exception);
    }

    #endregion

    #region RemoveStaffFromPatientTeam Tests

    [Fact]
    public void RemoveStaffFromPatientTeam_WithExistingConnection_RemovesConnection()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();
        _context.PatientStaffConnections.Add(new PatientStaffConnection
        {
            PatientId = patient.PatientId,
            StaffId = staff.StaffId
        });
        _context.SaveChanges();

        // Act
        _connectionService.RemoveStaffFromPatientTeam(patient.PatientId, staff.StaffId);

        // Assert
        var connection = _context.PatientStaffConnections
            .FirstOrDefault(c => c.PatientId == patient.PatientId && c.StaffId == staff.StaffId);
        Assert.Null(connection);
    }

    [Fact]
    public void RemoveStaffFromPatientTeam_WithNonExistingConnection_DoesNotThrow()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();
        // No connection exists

        // Act & Assert
        var exception = Record.Exception(() => 
            _connectionService.RemoveStaffFromPatientTeam(patient.PatientId, staff.StaffId));
        Assert.Null(exception);
    }

    [Fact]
    public void RemoveStaffFromPatientTeam_OnlyRemovesSpecifiedConnection()
    {
        // Arrange
        var patient = CreatePatient();
        var staff1 = CreateStaff("Dr", "Smith");
        var staff2 = CreateStaff("Jane", "Nurse");
        
        _context.PatientStaffConnections.AddRange(
            new PatientStaffConnection { PatientId = patient.PatientId, StaffId = staff1.StaffId },
            new PatientStaffConnection { PatientId = patient.PatientId, StaffId = staff2.StaffId }
        );
        _context.SaveChanges();

        // Act
        _connectionService.RemoveStaffFromPatientTeam(patient.PatientId, staff1.StaffId);

        // Assert
        var remainingConnections = _context.PatientStaffConnections
            .Where(c => c.PatientId == patient.PatientId)
            .ToList();
        Assert.Single(remainingConnections);
        Assert.Equal(staff2.StaffId, remainingConnections.First().StaffId);
    }

    [Fact]
    public void RemoveStaffFromPatientTeam_WithNonExistentPatient_DoesNotThrow()
    {
        // Arrange
        var staff = CreateStaff();
        var nonExistentPatientId = 99999;

        // Act & Assert
        var exception = Record.Exception(() => 
            _connectionService.RemoveStaffFromPatientTeam(nonExistentPatientId, staff.StaffId));
        Assert.Null(exception);
    }

    [Fact]
    public void RemoveStaffFromPatientTeam_WithNonExistentStaff_DoesNotThrow()
    {
        // Arrange
        var patient = CreatePatient();
        var nonExistentStaffId = 99999;

        // Act & Assert
        var exception = Record.Exception(() => 
            _connectionService.RemoveStaffFromPatientTeam(patient.PatientId, nonExistentStaffId));
        Assert.Null(exception);
    }

    #endregion

    #region Lifecycle Scenarios

    [Fact]
    public void AddThenRemove_ConnectionLifecycle_WorksCorrectly()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();

        // Act - Add
        _connectionService.AddStaffToPatientTeam(patient.PatientId, staff.StaffId);

        // Assert - Connection exists
        var connectionAfterAdd = _context.PatientStaffConnections
            .FirstOrDefault(c => c.PatientId == patient.PatientId && c.StaffId == staff.StaffId);
        Assert.NotNull(connectionAfterAdd);

        // Act - Remove
        _connectionService.RemoveStaffFromPatientTeam(patient.PatientId, staff.StaffId);

        // Assert - Connection removed
        var connectionAfterRemove = _context.PatientStaffConnections
            .FirstOrDefault(c => c.PatientId == patient.PatientId && c.StaffId == staff.StaffId);
        Assert.Null(connectionAfterRemove);
    }

    [Fact]
    public void AddAfterRemove_ReAddsConnection()
    {
        // Arrange
        var patient = CreatePatient();
        var staff = CreateStaff();

        // Act - Add, Remove, Add again
        _connectionService.AddStaffToPatientTeam(patient.PatientId, staff.StaffId);
        _connectionService.RemoveStaffFromPatientTeam(patient.PatientId, staff.StaffId);
        _connectionService.AddStaffToPatientTeam(patient.PatientId, staff.StaffId);

        // Assert - Connection exists again
        var connection = _context.PatientStaffConnections
            .FirstOrDefault(c => c.PatientId == patient.PatientId && c.StaffId == staff.StaffId);
        Assert.NotNull(connection);
    }

    #endregion

    public void Dispose()
    {
        _connectionService?.Dispose();
        _context?.Dispose();
    }
}
