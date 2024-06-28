using Appointments.Application.Common.Authorization;
using Appointments.Application.Common.Interfaces;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate;
using ErrorOr;
using MediatR;

namespace Appointments.Application.Offices.Commands;

[Authorize(Roles = AppRoles.Doctor)]
public record AddScheduleCommand(Guid OfficeId,  Dictionary<DateOnly, TimeRange> WorkingCalendar) : IRequest<ErrorOr<Success>>;

public class AddScheduleHandler : IRequestHandler<AddScheduleCommand, ErrorOr<Success>>
{
    private readonly IOfficeWriteRepository _officeWriteRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddScheduleHandler(IOfficeWriteRepository officeWriteRepository, ICurrentUserService currentUserService)
    {
        _officeWriteRepository = officeWriteRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Success>> Handle(AddScheduleCommand request, CancellationToken cancellationToken)
    {
        var office = await _officeWriteRepository.GetAsync(OfficeId.From(request.OfficeId), cancellationToken);

        if (office is null)
            return Error.NotFound();
        
        if (office.DoctorId.Value != _currentUserService.DoctorId)
            return Error.Unauthorized();
        
        office.AddSchedule(request.WorkingCalendar);

        await _officeWriteRepository.UpdateAsync(office, cancellationToken);
        
        return Result.Success;
    }
}