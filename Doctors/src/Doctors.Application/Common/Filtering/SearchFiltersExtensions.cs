using System.Diagnostics;
using Doctors.Domain.Common;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Doctors.Application.Common.Filtering;

public static class SearchFiltersExtensions
{
    public static IQueryable<T> ApplySearchFilters<T, TId>(this IQueryable<T> data, SearchFilters filters)
        where T : Entity<TId> 
    {
        if (!filters.SortBy.IsNullOrEmpty())
        {
            data = filters.SortOrder switch
            {
                EnSortOrder.Ascending => data.OrderBy(x => EF.Property<object>(x!, filters.SortBy!)).ThenBy(x => x.Id),
                EnSortOrder.Descending => data.OrderByDescending(x => EF.Property<object>(x!, filters.SortBy!))
                    .ThenBy(x => x.Id),
                _ => throw new UnreachableException()
            };
        }
        else
        {
            data = filters.SortOrder switch
            {
                EnSortOrder.Ascending => data.OrderBy(x => x.Id),
                EnSortOrder.Descending => data.OrderByDescending(x => x.Id),
                _ => throw new UnreachableException()
            };
        }
    
        return data.Skip((filters.PageNumber - 1) * filters.PageSize).Take(filters.PageSize);
    }
}