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
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Display(Name = "ZIP Code")]
        [DataType(DataType.PostalCode)]
        public string ZipCode { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}