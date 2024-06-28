using Appointments.Application.Common.Authorization;
using Appointments.Application.Common.Interfaces;
using Appointments.Application.Common.Models;
using Appointments.Domain.AppointmentAggregate;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Application.Appointments.Queries;

public record GetAppointmentQuery(Guid Id) : IRequest<ErrorOr<Appointment>>;

public class GetAppointmentHandler : IRequestHandler<GetAppointmentQuery, ErrorOr<Appointment>>
{
    private readonly IReadDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetAppointmentHandler(IReadDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Appointment>> Handle(GetAppointmentQuery request, CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.Appointments.AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id == AppointmentId.From(request.Id), cancellationToken);
        
        if (appointment is null || !DoesAppointmentBelongToUser(appointment))
            return Error.NotFound();
        
        return appointment;
    }
    
    private bool DoesAppointmentBelongToUser(Appointment appointment)
    {
        if(_currentUserService.User.IsPatient())
            return appointment.PatientId.Value == _currentUserService.PatientId;
        
        if(_currentUserService.User.IsDoctor())
            return appointment.DoctorId.Value == _currentUserService.DoctorId;
        
        return false;
    }
}