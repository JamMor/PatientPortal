using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PatientPortal.Models
{
    public class SeedFormView
    {
        public int CurrentPatients { get; set; }
        public int CurrentStaff { get; set; }

        [Display(Name = "Patients: ")]
        [Range(0,100)]
        public int Patients { get; set; }
        
        [Display(Name = "Staff: ")]
        [Range(0,10)]
        public int Staff { get; set; }
    }

}