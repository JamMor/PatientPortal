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

        public ListResultAttributes DisplayProperties { get; set; }

        public List<PatientResult> SearchResults { get; set; }
    }

    public class PatientResult
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        public int Age { get; set; }
        public string Last4SSN { get; set; }
    }
}