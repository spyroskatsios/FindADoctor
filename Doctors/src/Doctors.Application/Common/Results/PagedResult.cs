namespace Doctors.Application.Common.Results;

public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int PageNumber, int PageSize);
    //int TotalPages, bool HasPrevious, bool HasNext);