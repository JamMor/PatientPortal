using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    public class Address
    {
        [Key]
        [ForeignKey("Patient")]
        public int AddressId { get; set; }

        [Display(Name = "Street Address")]
        public required string StreetAddress { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }

        [Display(Name = "ZIP Code")]
        [DataType(DataType.PostalCode)]
        public required string ZipCode { get; set; }

        public Patient? Patient { get; set; }
    }
}
