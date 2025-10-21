using System.Collections.Generic;
using System.Linq;

namespace PatientPortal.Models
{
    public record SortableColumn(string DisplayName, string PropertyName);

    public class SortState
    {
        private readonly string _column;
        private readonly string _direction;

        public SortState(SortableColumn[] columns, string defaultSort, string inputSortString)
        {
            var parts = (inputSortString ?? defaultSort).Split('_', 2);
            var column = parts[0];
            var direction = parts.Length == 2 && parts[1] == "asc" ? "asc" : "desc";

            SortString = columns.Any(c => c.PropertyName == column)
                ? $"{column}_{direction}"
                : defaultSort;

            var validatedParts = SortString.Split('_', 2);
            _column = validatedParts[0];
            _direction = validatedParts[1];
        }

        /// <summary>
        /// The sort string to be used in links and passed to the service layer,
        /// guaranteed to be valid based on the provided columns and default sort.
        /// </summary>
        public string SortString { get; }

        public bool IsSortedBy(SortableColumn column) => IsSortedBy(column.PropertyName);
        public bool IsSortedBy(string propertyName) => _column == propertyName;

        public bool IsAscending(SortableColumn column) => IsAscending(column.PropertyName);
        public bool IsAscending(string propertyName) => IsSortedBy(propertyName) && _direction == "asc";

        /// <summary>
        /// Returns the appropriate sort string parameter for a sort column toggle:
        /// if currently sorted by that column, toggles direction; if not,
        /// defaults to descending.
        /// </summary>
        public string GetSortOrderForLink(SortableColumn column) => GetSortOrderForLink(column.PropertyName);
        public string GetSortOrderForLink(string propertyName) =>
            _column == propertyName
                ? $"{propertyName}_{(_direction == "desc" ? "asc" : "desc")}"
                : $"{propertyName}_desc";

        public Dictionary<string, string> ToRouteDict() =>
            new Dictionary<string, string> { { "SortOrder", SortString } };
    }
}
