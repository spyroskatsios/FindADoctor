using Doctors.Application.Common.Interfaces;
using Doctors.Domain.OfficeAggregate;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Doctors.Application.Offices.Queries;

public record GetOfficesQuery(Guid DoctorId) : IRequest<ErrorOr<List<Office>>>;

public class GetOfficesQueryHandler : IRequestHandler<GetOfficesQuery, ErrorOr<List<Office>>>
{
    private readonly IReadDbContext _readDbContext;

    public GetOfficesQueryHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<ErrorOr<List<Office>>> Handle(GetOfficesQuery request, CancellationToken cancellationToken)
    {
        if (!await _readDbContext.Doctors.AnyAsync(x => x.Id.Value == request.DoctorId, cancellationToken))
            return Error.NotFound(description: "Doctor not found");

        return await _readDbContext.Offices.Where(x => x.DoctorId.Value == request.DoctorId)
            .ToListAsync(cancellationToken);
    }
}