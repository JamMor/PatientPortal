using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class PatientManagerView
    {
        public PatientSearch SearchBar { get; set; }
        
        public List<Patient> SearchResults { get; set; }
    }
}