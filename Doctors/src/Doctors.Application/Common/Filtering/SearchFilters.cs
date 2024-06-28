namespace Doctors.Application.Common.Filtering;

public record SearchFilters
{
    public string? SortBy { get; init; } 
    public EnSortOrder SortOrder { get; init; } 
    public int PageSize { get; init; } 
    public int PageNumber { get; init; } 

    protected SearchFilters(int pageSize, int pageNumber, EnSortOrder sortOrder, string? sortBy = null )
    {
        PageSize = pageSize switch
        {
            > 100 => 100,
            < 1 => 1,
            _ => pageSize
        };
        
        PageNumber = pageNumber < 1 
            ? 1 
            : pageNumber;
        
        SortBy = sortBy;
        SortOrder = sortOrder;
    }
}