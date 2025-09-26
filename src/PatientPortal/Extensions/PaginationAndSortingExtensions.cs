using System.Collections.Generic;
using System.Linq;

namespace PatientPortal.Extensions
{
    public static class PaginationExtensions
    {
        public static List<T> ToPagedList<T>(this IQueryable<T> query, int resultsPerPage, int currentPage)
        {
            return query
                .Skip(resultsPerPage*(currentPage-1))
                .Take(resultsPerPage)
                .ToList();
        }
    }
}