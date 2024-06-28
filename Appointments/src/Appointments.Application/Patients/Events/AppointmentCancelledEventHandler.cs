using Appointments.Application.Common.Events;
using Appointments.Application.Common.Repositories;
using Appointments.Domain.AppointmentAggregate;
using MediatR;
using Throw;

namespace Appointments.Application.Patients.Events;

public class AppointmentCancelledEventHandler : INotificationHandler<DomainEventNotification<AppointmentCancelledEvent>>
{
    private readonly IPatientWriteRepository _patientWriteRepository;

    public AppointmentCancelledEventHandler(IPatientWriteRepository patientWriteRepository)
    {
        _patientWriteRepository = patientWriteRepository;
    }

    public async Task Handle(DomainEventNotification<AppointmentCancelledEvent> notification, CancellationToken cancellationToken)
    {
        var patient = await _patientWriteRepository.GetAsync(notification.DomainEvent.Appointment.PatientId, cancellationToken);

        patient.ThrowIfNull();
        
        patient.RemoveAppointment(notification.DomainEvent.Appointment);
        
        await _patientWriteRepository.UpdateAsync(patient, cancellationToken);
    }
}