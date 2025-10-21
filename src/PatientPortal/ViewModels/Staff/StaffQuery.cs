using System.Collections.Generic;

namespace PatientPortal.Models
{
    public class StaffQuery
    {
        public const string DefaultSort = "LastName_asc";

        private static readonly SortableColumn[] _sortableColumns =
        [
            new("Last Name", "LastName"),
            new("Role", "Role"),
            new("Staff Id", "StaffId"),
        ];
        public SortableColumn[] SortableColumns => _sortableColumns;

        public static StaffQuery Create(StaffFilter filter, Paginator paging, string sortOrder) =>
            new()
            {
                Filter = filter,
                Sort = new SortState(_sortableColumns, DefaultSort, sortOrder),
                Paging = paging,
            };

        public required StaffFilter Filter { get; set; }
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
