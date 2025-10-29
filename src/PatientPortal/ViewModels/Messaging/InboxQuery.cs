using System.Collections.Generic;

namespace PatientPortal.Models
{
    public class InboxQuery
    {
        public required InboxType Type { get; init; }
        public required bool OnlyUnread { get; init; }
        public required Paginator Paging { get; init; }

        public Dictionary<string, string> ToRouteDict()
        {
            var dict = new Dictionary<string, string>
            {
                { "inbox", Type.Route },
                { "ResultsPerPage", Paging.ResultsPerPage.ToString() },
                { "CurrentPage", Paging.CurrentPage.ToString() },
            };
            if (OnlyUnread)
                dict["OnlyUnread"] = "true";
            return dict;
        }
    }
}
