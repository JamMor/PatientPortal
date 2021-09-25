using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class StaffManagerView
    {
        public StaffSearch SearchBar { get; set; }
        
        public List<Staff> SearchResults { get; set; }
    }
}