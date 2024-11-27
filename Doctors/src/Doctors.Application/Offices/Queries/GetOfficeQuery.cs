using Doctors.Application.Common.Interfaces;
using Doctors.Domain.OfficeAggregate;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Doctors.Application.Offices.Queries;

public record GetOfficeQuery(Guid OfficeId) : IRequest<ErrorOr<Office>>;

public class GetOfficeHandler : IRequestHandler<GetOfficeQuery, ErrorOr<Office>>
{
    private readonly IReadDbContext _readDbContext;

    public GetOfficeHandler(IReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    public async Task<ErrorOr<Office>> Handle(GetOfficeQuery request, CancellationToken cancellationToken)
    {
        var office = await _readDbContext.Offices.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == OfficeId.From(request.OfficeId), cancellationToken);

        if (office is null)
            return Error.NotFound("Office not found");

        return office;
    }
}