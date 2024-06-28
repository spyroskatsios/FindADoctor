using Appointments.Application.Common.Authorization;
using Appointments.Application.Common.Interfaces;
using Appointments.Application.Common.Models;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.AppointmentAggregate;
using ErrorOr;
using MediatR;

namespace Appointments.Application.Appointments.Commands;

public record CancelAppointmentCommand(Guid AppointmentId) : IRequest<ErrorOr<Success>>;

public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentCommand, ErrorOr<Success>>
{
    private readonly IAppointmentWriteRepository _appointmentWriteRepository;
    private readonly ICurrentUserService _currentUserService;

    public CancelAppointmentHandler(IAppointmentWriteRepository appointmentWriteRepository, ICurrentUserService currentUserService)
    {
        _appointmentWriteRepository = appointmentWriteRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Success>> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointmentWriteRepository.GetAsync(AppointmentId.From(request.AppointmentId), cancellationToken);
        
        if (appointment is null || !DoesAppointmentBelongToUser(appointment))
            return Error.NotFound();
        
        var result = appointment.Cancel();
        
        if (result.IsError)
            return result.Errors;
        
        await _appointmentWriteRepository.UpdateAsync(appointment, cancellationToken);
        return Result.Success;
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