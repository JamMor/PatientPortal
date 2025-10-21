using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class PatientManagerView
    {
        public required PatientQuery Query { get; set; }

        public required PatientResultList Results { get; set; }

        public PatientSearchForm SearchForm => new PatientSearchForm
        {
            PatientId = Query.Filter.PatientId,
            FirstName = Query.Filter.FirstName,
            LastName = Query.Filter.LastName,
            SSN = Query.Filter.SSN,
            Birthdate = Query.Filter.Birthdate,
            OnlyPatientsUnderCare = Query.Filter.OnlyPatientsUnderCare,
            SortOrder = Query.Sort.SortString,
            ResultsPerPage = Query.Paging.ResultsPerPage,
            CurrentPage = Query.Paging.CurrentPage,
        };
    }

    [NotMapped]
    public class PatientResultList
    {
        public List<PatientResult> Patients { get; set; } = [];
    }

    public class PatientResult
    {
        public int PatientId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DOB { get; set; }
        public int Age { get; set; }
        public required string Last4SSN { get; set; }
    }
}
