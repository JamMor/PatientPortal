#nullable enable
using System;
using System.Collections.Generic;

namespace PatientPortal.Models
{
    public class PatientInfoViewModel
    {
        public required PatientHeaderInfoView PatientHeader { get; set; }
        public int PatientId { get; set; }
        public int MessagingLinkId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DOB { get; set; }
        public required string Last4SSN { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int Age { get; set; }

        public string FullName()
        {
            return FirstName + " " + LastName;
        }

        public AddressInfo? Address { get; set; }
        public List<StaffInfo> MedicalTeam { get; set; } = [];
        public List<HealthIssueInfo> HealthIssues { get; set; } = [];
        public List<VisitInfo> Visits { get; set; } = [];
        public List<TestResultInfo> TestResults { get; set; } = [];
    }

    public class AddressInfo
    {
        public required string StreetAddress { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
    }

    public class StaffInfo
    {
        public int StaffId { get; set; }
        public required string FullName { get; set; }
        public required string Role { get; set; }
    }

    public class HealthIssueInfo
    {
        public int HealthIssueId { get; set; }
        public required string ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int AssociatedVisitsCount { get; set; }
        public int AssociatedTestResultsCount { get; set; }
    }

    public class VisitInfo
    {
        public int VisitId { get; set; }
        public required string Comment { get; set; }
        public DateTime DateOfVisit { get; set; }
        public required string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class TestResultInfo
    {
        public int TestResultId { get; set; }
        public required string Type { get; set; }
        public required string Comment { get; set; }
        public required string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
