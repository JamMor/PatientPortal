using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class NewPatientInput
    {
        public Patient Patient { get;set; }
        public Address Address { get;set; }

    }
}