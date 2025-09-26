using System;

namespace PatientPortal.Models
{
    public class Paginator
    {
        public int ResultsCount { get; set; }
        public int CurrentPage { get; set; } = 1;

        public int ResultsPerPage { get; set; } = 10;
        public int TotalPages()
        {
            return (int)Math.Ceiling(ResultsCount / (double)ResultsPerPage);
        }
        // # of page links displayed in navbar at a time
        public int MaxPageLinksPerPage { get; } = 5;

        public int StartPage()
        {
            int offset = (int)Math.Floor(MaxPageLinksPerPage / (double)2.0);

            if (CurrentPage - offset <= 1)
            {
                return 1;
            }
            else if (CurrentPage + offset > TotalPages())
            {
                int sPage = TotalPages() - MaxPageLinksPerPage + 1;
                return sPage > 0 ? sPage : 1;
            }
            else
            {
                return CurrentPage - offset;
            }
        }

        public string SortOrder { get; set; } = "LastName_asc";
        public string SortColumn()
        {
            return SortOrder.Split("_")[0];
        }

        public string SortDirection()
        {
            return SortOrder.Split("_")[1];
        }
        public string Reverse()
        {
            return SortDirection() == "desc" ? "asc" : "desc";
        }
    }
}