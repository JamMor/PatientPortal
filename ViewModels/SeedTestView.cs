using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PatientPortal.Models
{
    [NotMapped]
    public class SeedTestView
    {

        [Display(Name = "Patients: ")]
        [Range(0,100)]
        public int Patients { get; set; }
        
        [Display(Name = "Staff: ")]
        [Range(0,10)]
        public int Staff { get; set; }
    }

}