using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class StaffManagerView
    {
        public required StaffQuery Query { get; set; }

        public required StaffResultList Results { get; set; }

        public StaffSearchForm SearchForm => new StaffSearchForm
        {
            StaffId = Query.Filter.StaffId,
            FirstName = Query.Filter.FirstName,
            LastName = Query.Filter.LastName,
            Role = Query.Filter.Role,
            SortOrder = Query.Sort.SortString,
            ResultsPerPage = Query.Paging.ResultsPerPage,
            CurrentPage = Query.Paging.CurrentPage,
        };
    }

    [NotMapped]
    public class StaffResultList
    {
        public List<StaffResult> Staff { get; set; } = [];
    }

    public class StaffResult
    {
        public int StaffId { get; set; }
        public required string FullName { get; set; }
        public required string Role { get; set; }
        public int PatientCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
