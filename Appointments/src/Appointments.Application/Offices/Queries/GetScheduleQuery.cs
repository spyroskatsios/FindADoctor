using Appointments.Application.Common.Interfaces;
using Appointments.Domain.Common.ValueObjects;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Application.Offices.Queries;

public record GetScheduleQuery(Guid OfficeId) : IRequest<ErrorOr<Dictionary<DateOnly, List<TimeRange>>>>;

public class GetScheduleHandler : IRequestHandler<GetScheduleQuery, ErrorOr<Dictionary<DateOnly, List<TimeRange>>>>
{
    private readonly IReadDbContext _dbContext;

    public GetScheduleHandler(IReadDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Dictionary<DateOnly, List<TimeRange>>>> Handle(GetScheduleQuery request, CancellationToken cancellationToken)
    {
        var office =  await _dbContext.Offices
            .FirstOrDefaultAsync(x => x.Id.Value == request.OfficeId, cancellationToken);

        if (office is null)
            return Error.NotFound();
        
        return office.GetAvailableTimeSlots();
    }
}