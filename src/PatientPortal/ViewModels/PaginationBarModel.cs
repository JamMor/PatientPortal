using System.Collections.Generic;

namespace PatientPortal.Models
{
    public class PaginationBarModel
    {
        public required Paginator Paging { get; set; }
        public required Dictionary<string, string> Routes { get; set; }
    }
}
