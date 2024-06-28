using Doctors.Application.Common.Filtering;
using Doctors.Application.Common.Interfaces;
using Doctors.Application.Common.Results;
using Doctors.Domain.DoctorAggregate;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Doctors.Application.Doctors.Queries;

public record SearchDoctorsQuery(string? FirstName, string? LastName , Speciality? Speciality,
    int PageSize, int PageNumber, EnSortOrder SortOrder, string? SortBy)
    : SearchFilters(PageSize, PageNumber, SortOrder, SortBy),
      IRequest<ErrorOr<PagedResult<Doctor>>>;

public class SearchDoctorsHandler : IRequestHandler<SearchDoctorsQuery, ErrorOr<PagedResult<Doctor>>>
{
    private readonly IReadDbContext _readDbContext;

    public SearchDoctorsHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<ErrorOr<PagedResult<Doctor>>> Handle(SearchDoctorsQuery request, CancellationToken cancellationToken)
    {
        var doctorsQueryable = _readDbContext.Doctors.AsNoTracking()
            .ApplyDoctorFilters(request);
        
        return new PagedResult<Doctor>(
            await doctorsQueryable.ApplySearchFilters<Doctor, DoctorId>(request).ToListAsync(cancellationToken),
            await doctorsQueryable.CountAsync(cancellationToken), 
            request.PageNumber, 
            request.PageSize);
    }
}

