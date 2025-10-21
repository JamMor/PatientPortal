// Form Input Model
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models
{
    [NotMapped]
    public class StaffSearchForm : StaffFilter
    {
        // Sorting
        public string SortOrder { get; set; } = "LastName_asc";

        // Pagination
        public int ResultsPerPage { get; set; } = 10;
        public int CurrentPage { get; set; } = 1;
    }
}
