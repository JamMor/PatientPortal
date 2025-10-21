using System.Collections.Generic;

namespace PatientPortal.Models
{
    public class PatientQuery
    {
        public const string DefaultSort = "LastName_asc";

        private static readonly SortableColumn[] _sortableColumns =
        [
            new("Last Name", "LastName"),
            new("DOB", "DOB"),
            new("Patient Id", "PatientId"),
        ];
        public SortableColumn[] SortableColumns => _sortableColumns;

        public static PatientQuery Create(PatientFilter filter, Paginator paging, string sortOrder) =>
            new()
            {
                Filter = filter,
                Sort = new SortState(_sortableColumns, DefaultSort, sortOrder),
                Paging = paging,
            };

        public required PatientFilter Filter { get; set; }
        public required SortState Sort { get; set; }
        public required Paginator Paging { get; set; }

        public Dictionary<string, string> ToRouteDict()
        {
            var dict = new Dictionary<string, string>(Filter.ToRouteDict());
            foreach (var kv in Sort.ToRouteDict())
                dict[kv.Key] = kv.Value;
            foreach (var kv in Paging.ToRouteDict())
                dict[kv.Key] = kv.Value;
            return dict;
        }
    }
}
