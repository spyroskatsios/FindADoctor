namespace Doctors.Contracts.Common;

public record PagedResponse<T>(IEnumerable<T> Items, int TotalCount, int CurrentPage, int PageSize, 
    int TotalPages, bool HasPrevious, bool HasNext);