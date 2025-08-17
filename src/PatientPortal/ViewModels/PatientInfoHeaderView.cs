#nullable enable
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class PatientHeaderInfoView
    {
        public int CurrentPatientId { get; set; }
        public int? CurrentPatientLinkId { get; set; }
        public required string CurrentPatientFirstName { get; set; }
        public required string CurrentPatientLastName { get; set; }
        public required string CurrentPatientSSN { get; set; }
        public DateTime CurrentPatientDOB { get; set; }
        public int CurrentPatientAge { get; set; }
        public DateTime CurrentPatientCreatedOn { get; set; }

        public string CurrentPatientFullName
        {
            get => $"{CurrentPatientFirstName} {CurrentPatientLastName}";
        }

        public string CurrentPatientFullNameReverse
        {
            get => $"{CurrentPatientLastName}, {CurrentPatientFirstName}";
        }
    }
}
