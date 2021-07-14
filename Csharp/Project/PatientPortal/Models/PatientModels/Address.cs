using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        
        [Required]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        [Display(Name = "ZIP Code")]
        [DataType(DataType.PostalCode)]
        public int ZipCode { get; set; }

        [Required]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
    }
}