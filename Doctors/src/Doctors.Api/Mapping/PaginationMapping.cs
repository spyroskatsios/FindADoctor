using Doctors.Application.Common.Results;
using Doctors.Contracts.Common;

namespace Doctors.Api.Mapping;

public static class PaginationMapping
{
    public static PagedResponse<TOut> ToPagedResponse<TOut, TIn>(this PagedResult<TIn> result, Func<TIn, TOut> map)
    {
        var totalPages = (int)Math.Ceiling(result.TotalCount / (double)result.PageSize);
        return new PagedResponse<TOut>(result.Items.Select(map), result.TotalCount, result.PageNumber, 
            result.PageSize, totalPages, result.PageNumber > 1, result.PageNumber < totalPages);
    }
        
}